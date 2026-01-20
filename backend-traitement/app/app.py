from flask import Flask
from controllers.filesController import filesController

app = Flask(__name__)

# Controllers
app.register_blueprint(filesController)
