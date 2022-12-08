#coding=utf-8
from fastapi import FastAPI, Request
import time
from starlette.websockets import WebSocket
import uvicorn
import threading
import sys
import pickle
from os.path import exists

app = FastAPI()
from fastapi.middleware.cors import CORSMiddleware

origins = ["*"]

app.add_middleware(
	CORSMiddleware,
	allow_origins=origins,
	allow_credentials=True,
	allow_methods=["*"],
	allow_headers=["*"],
)

online_players = {}
try:
	with open("recent_ips.pickle", "rb") as file:
		online_players = pickle.load(file)
except:
	print("errer: no addresses found in pickle")

def check_online_players():
	global online_players
	while True:
		current_time = time.time()
		new_online_players = {}
		for (key, value) in online_players.items():
			if current_time - value < 86400: #more than 24 hour old ip addresses will be removed
				new_online_players[key] = value

		online_players = new_online_players
		with open("recent_ips.pickle", "wb") as file:
			pickle.dump(online_players, file)

		time.sleep(10)

threading.Thread(target=check_online_players).start()

@app.get("/add_online_player")
async def add_online_player(ip):
	global online_players
	online_players[ip] = time.time()
	
	return "success"

@app.get("/online_players") #only on the official server to prevent using different time zone
async def return_players(request:Request):
	global online_players
	return len(online_players) 

@app.get("/testing") #only on the official server to prevent using different time zone
async def test(request:Request):
	return str(time.time()) + ", " + str(request.client.host)

#https
uvicorn.run(app, port=9999, host="0.0.0.0", ssl_keyfile="/etc/letsencrypt/live/redacted/redacted.pem", ssl_certfile="/etc/letsencrypt/live/redacted/redacted.pem")


