import http.server
import mimetypes
import os
import socketserver
from pathlib import Path


HOST = os.environ.get("HOST", "127.0.0.1")
PORT = int(os.environ.get("PORT", "4174"))
ROOT = Path(__file__).resolve().parent

mimetypes.add_type("text/javascript", ".mjs")


class Handler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=str(ROOT), **kwargs)

    def end_headers(self):
        self.send_header("Cache-Control", "no-store")
        super().end_headers()


with socketserver.TCPServer((HOST, PORT), Handler) as server:
    print(f"Arayüz: http://{HOST}:{PORT}/index.html")
    server.serve_forever()
