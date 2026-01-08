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
    
    return Response(
        service.read_csv_and_stream(file),
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
