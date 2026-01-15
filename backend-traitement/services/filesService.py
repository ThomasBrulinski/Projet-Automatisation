import csv
import io
import json
import requests
import hashlib
import os

class FilesService:
    def read_csv_and_stream(self, file, batch_size=50):
        def generate_row_hash(row):
            # On choisit les colonnes qui définissent l'unicité
            # On concatène les valeurs en une seule chaîne
            unique_string = f"{row.get('Sub job ID')}-{row.get('Source ID')}-{row.get('Destination ID')}-{row.get('Migration start time')}"
                
            # On crée un hash SHA-256 (plus robuste que MD5)
            return hashlib.sha256(unique_string.encode('utf-8')).hexdigest()

        try:
            # 1. Lecture immédiate pour éviter le "closed file"
            file_content = file.stream.read().decode("utf-8-sig")
            stream = io.StringIO(file_content, newline=None)
            reader_obj = csv.DictReader(stream)
            reader = list(reader_obj)
            
            # 2. Mapping : On transforme TOUTES les lignes avec les noms C#
            formatted_results = []
            for row in reader:
                clean_date = None
                raw_date = row.get("Migration start time")
                row_hash = generate_row_hash(row)
                if raw_date:
                    try:
                        # 1. On sépare à la parenthèse : ["2025-11-27 15:26:41 ", "UTC+01:00)"]
                        # 2. On prend le premier élément [0]
                        # 3. .strip() enlève l'espace restant à la fin
                        date_part = raw_date.split("(")[0].strip()
                        
                        # 4. On remplace l'espace du milieu par 'T' pour le format ISO C#
                        clean_date = date_part.replace(" ", "T")
                    except Exception as e:
                        # En cas de format imprévu, on laisse clean_date à None
                        # pour éviter de bloquer l'import total
                        clean_date = None

                formatted_results.append({
                    "RowHash": row_hash,
                    "MigrationStartTime": clean_date,
                    "SubJobId": row.get("Sub job ID"),
                    "Title": row.get("Title"),
                    "Type": row.get("Type"),
                    "SourceId": row.get("Source ID"),
                    "Source": row.get("Source"),
                    "DestinationId": row.get("Destination ID"),
                    "Destination": row.get("Destination"),
                    "Size": row.get("Size"),
                    "Status": row.get("Status"),
                    "MigrationAction": row.get("Migration action"),
                    "Comment": row.get("Comment"),
                    "ErrorCode": row.get("Error code")
                })
            
            total_lines = len(formatted_results)
            csharp_url = os.environ.get('C_SHARP_URL')

            if not csharp_url:
                raise Exception(".env non configuré")
            
            def generate():
                if total_lines == 0:
                    yield json.dumps({"progress": 100, "message": "Fichier vide"}) + "\n"
                    return
                
                total_inserted = 0
                total_skipped = 0

                # 3. On itère sur la liste déjà formatée
                for i in range(0, total_lines, batch_size):
                    batch = formatted_results[i : i + batch_size]
                    
                    try:
                        r = requests.post(csharp_url, json=batch, timeout=5)

                        # --- RÉCUPÉRATION DES STATS DU C# ---
                        response_data = r.json() 
                        stats = response_data.get("data", {})
                        total_inserted += stats.get("inserted", 0)
                        total_skipped += stats.get("skipped", 0)
                        # ------------------------------------
                        
                        # Si le C# renvoie une erreur 400 (Validation), on capture le détail
                        if r.status_code == 400:
                            yield json.dumps({
                                "error": "Données invalides pour C#", 
                                "details": r.text # Contient les erreurs de validation précises
                            }) + "\n"
                            return
                            
                        r.raise_for_status()
                    except Exception as e:
                        yield json.dumps({"error": "Échec transfert C#", "details": str(e)}) + "\n"
                        return 

                    progress = min(100, int((i + batch_size) / total_lines * 100))
                    yield json.dumps({"progress": progress}) + "\n"
                    
                yield json.dumps({
                    "progress": 100, 
                    "complete": True,
                    "inserted": total_inserted, 
                    "skipped": total_skipped,
                    "total": total_lines
                }) + "\n"
            
            return generate()

        except Exception as e:
            def error_gen():
                yield json.dumps({"error": "Erreur de lecture fichier", "details": str(e)}) + "\n"
            return error_gen()
    
    def load_data_from_db(self, page, query):
        base_url = os.environ.get('C_SHARP_URL')
        if not base_url:
            raise Exception(".env non configuré")
        
        params = {
            'page': page,
            'query': query
        }

        try:
            r = requests.get(base_url, params=params, timeout=10)
            r.raise_for_status()
            return r.json() # On renvoie la liste Python brute
        except requests.exceptions.RequestException as e:
            print(f"Erreur : {e}")
            return [] # On renvoie une liste vide propre
