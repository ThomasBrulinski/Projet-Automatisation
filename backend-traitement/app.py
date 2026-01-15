from flask import Flask
from flask_cors import CORS
from controllers.filesController import filesController

app = Flask(__name__)
CORS(app, origins=["http://localhost:8080"])

# Controllers
app.register_blueprint(filesController)
