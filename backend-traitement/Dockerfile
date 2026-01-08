FROM python:3.13.1-alpine

WORKDIR /app

RUN apk add --no-cache gcc musl-dev python3-dev libffi-dev openssl-dev mariadb-connector-c-dev

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

RUN chmod +x run.sh
EXPOSE 8000

CMD ["./run.sh"]
