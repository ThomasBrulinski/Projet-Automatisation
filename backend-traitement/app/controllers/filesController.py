from flask import Blueprint, request, Response
from services.filesService import FilesService
import json


filesController = Blueprint('filesController', __name__)
service = FilesService() 

@filesController.route('/api/files/', methods=["POST"])
def upload_csv():
    if 'file' not in request.files:
        return {"error": "No file"}, 400
    
    file = request.files['file']
    
    # Vérifier si le premier chunk contient une erreur
    generator = service.read_csv_and_stream(file)
    first_chunk_str = None
    
    try:
        first_chunk_str = next(generator)
        first_chunk = json.loads(first_chunk_str)
        
        if "error" in first_chunk:
            # Erreur détectée, retourner 400
            return {"error": first_chunk.get("error"), "details": first_chunk.get("details")}, 400
    except StopIteration:
        pass
    except json.JSONDecodeError:
        pass
    
    # Sinon, streamer normalement
    def stream_result():
        if first_chunk_str:
            yield first_chunk_str
        for chunk in generator:
            yield chunk
    
    return Response(
        stream_result(),
        mimetype='application/json'
    )

@filesController.route('/api/files/', methods=["GET"])
def load_data_from_db_route(): 
    page = request.args.get('page', default=0, type=int)
    query = request.args.get('query', default="", type=str)
    data = service.load_data_from_db(page, query)
    return Response(
        json.dumps(data), 
        mimetype='application/json'
    )
