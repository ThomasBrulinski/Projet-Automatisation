#!/bin/sh
exec gunicorn -w 4 --threads 4 -b 0.0.0.0:8000 app:app
