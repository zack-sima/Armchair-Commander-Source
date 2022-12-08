#coding=utf-8
from fastapi import FastAPI, Form, File
import threading
import time
from starlette.websockets import WebSocket
import uvicorn
import random
import sys

app = FastAPI()

# from fastapi.middleware.cors import CORSMiddleware
# origins = [
#     "*"
# ]
# app.add_middleware(
#     CORSMiddleware,
#     allow_origins=origins,
#     allow_credentials=True,
#     allow_methods=["*"],
#     allow_headers=["*"],
# )

class Map:
    def __init__(self, map_id: int, player_countries, map_data):
        self.player_countries = player_countries #list of countries; length of this list is the limit to players
        self.map_id = map_id
        self.map_data = map_data #optional

class Room:
    def __init__(self, map_details: Map, matching: bool):
        self.player_names = {} #initially planned for only 2 people, but framework should be made to allow for more
        self.player_timeouts = {} #time left before player is timed out (this is reset everytime a connection is passed through)
        self.map_details = map_details
        self.current_player = 0 #the player having the turn
        self.map_json = "" #json is uploaded by players
        self.map_view_only = False #if view only new turn will not begin after retrieving this map (for opponents to update map during other player's turn)
        self.random_id = 0
        self.game_started = False
        self.matching = matching #searchable on public
    def return_value(self):
        return ClientRoom(self)

class ClientRoom:
    def __init__(self, room: Room):
        self.player_names = []
        for i, j in room.player_names.items():
            self.player_names.append(j)
        self.player_timeouts = []
        for i, j in room.player_timeouts.items():
            self.player_timeouts.append(j)
        self.player_countries = room.map_details.player_countries
        self.map_id = room.map_details.map_id
        self.map_data = room.map_details.map_data
        self.current_player = room.current_player
        self.random_id = room.random_id
        self.map_view_only = room.map_view_only

def add_player_to_room(room_id: int, player_id: int, player_name: str):
    rooms[room_id].player_names[player_id] = player_name
    #players are given something like 100 seconds per round; local game will automatically finish round once time is up
    rooms[room_id].player_timeouts[player_id] = 25 #very little amount of seconds tolerated before player is kicked

def kick_player_from_room(room_id: int, player_id: int):
    #delete player
    try:
        del rooms[room_id].player_names[player_id]
        del rooms[room_id].player_timeouts[player_id]
    except:
        print("could not kick")
    

rooms = {} #key: room id, value: Room class

def rooms_idle_countdown():
    ticker = threading.Event()
    while not ticker.wait(1):
        try:
            for i, j in rooms.items():
                #index is player id; i is room id
                for index, h in j.player_timeouts.items():
                    j.player_timeouts[index] -= 1
                    if j.player_timeouts[index] < 0:
                        print("kicked player #" + str(index) + " from game due to connection")
                        kick_player_from_room(i, index)
                        break
                #delete room if it is empty
                if len(j.player_names) == 0:
                    del rooms[i]
                    print("deleted room")
                    break
        except Exception as e:
            print(str(e))

threading.Thread(target=rooms_idle_countdown).start()

#call this to kick player immediately
@app.get("/leave_game")
async def leave_game(room_id: int, player_id: int):
    try:
        kick_player_from_room(room_id, player_id)
        if len(rooms[room_id].player_names) == 0:
            del rooms[room_id]
            print("deleted room by player leaving")

        return 0
    except:
        return -1


map_ids = [
150, 152, 153, 154, 155, 
156, 157, 158, 159, 160, 
161, 162, 163, 164, 165, 166
]
ally_countries = [
["USA"],
["France", "UK", "NeutralSoviet"],
["France", "UK", "Soviet"],
["France", "NeutralSoviet", "Hungary"],
["ROC"],

["Soviet"],
["France", "Soviet", "UK"],
["Norway"],
["France", "UK", "USA"],
["PRC"],

["Soviet"],
["EastGermany", "Soviet"],
["PRCNew"],
["Soviet"],
["Soviet"],
["UK"]

]
axis_countries = [
["German"],
["German", "Italy"],
["German", "Italy"],
["German", "UK"],
["Japan"],

["German"],
["German"],
["Finland"],
["German", "Japan", "FascistSpain"],
["NatoROC"],

["NatoUSA"],
["WestGermany", "USA", "NatoFrance"],
["NatoUSA"],
["PRCNew"],
["German"],
["German"]





]
@app.post("/custom_match")
async def host_custom(player_name: str = Form(...), map_data:str = Form(...), player_country:str = Form(...), opponent_country: str = Form(...)):
    found_room = None #if this stays null a new room will be created
    
    if found_room == None:
        new_room_id = random.randint(100000, 10000000)
        while rooms.get(new_room_id) is not None:
            new_room_id = random.randint(100000, 10000000)

        random_map = random.randint(0, len(axis_countries) - 1)

        rooms[new_room_id] = Room(Map(0, [player_country, opponent_country], map_data), False)

        add_player_to_room(new_room_id, 0, player_name)

        #returns newly created
        print(str((new_room_id, 0)) + "1")
        return (new_room_id, 0)

#assign player to the waiting room; player will check the room to see if player has been matched every few seconds
#a new room will be created if none are available
@app.get("/assign_match")
async def assign_room(player_name: str, matching: bool): #if not matching it is a custom room
    found_room = None #if this stays null a new room will be created
    if matching: #will create a new room if not matching
        for i, j in rooms.items():
            if not j.game_started and len(j.player_names) < len(j.map_details.player_countries):
                add_player_to_room(i, len(j.player_names), player_name) #because length of players dictionary is used, the pairs must be immedietely removed upon exiting the game
                found_room = j

                #returns room
                print((i, len(j.player_names) - 1))
                return (i, len(j.player_names) - 1)

    if found_room == None:
        new_room_id = random.randint(100000, 10000000)
        while rooms.get(new_room_id) is not None:
            new_room_id = random.randint(100000, 10000000)
        # ally_countries = ["France", "UK", "Soviet"]
        # axis_countries = ["Italy", "German"]

        random_map = random.randint(0, len(axis_countries) - 1)


        rooms[new_room_id] = Room(Map(map_ids[random_map], [axis_countries[random_map][random.randint(0, len(axis_countries[random_map]) - 1)], ally_countries[random_map][random.randint(0, len(ally_countries[random_map]) - 1)]], ""), matching) #no custom map given
        print(rooms[new_room_id].map_details.player_countries)
        add_player_to_room(new_room_id, 0, player_name)
        #returns newly created
        print(str((new_room_id, 0)) + "1")
        return (new_room_id, 0)

@app.get("/list_matches") #just to see all the matches; maybe implement a browse rooms mode
async def list_matches():
    return len(rooms)

@app.get("/join_match") #custom match
async def join_match(player_name: str, room_id: int):
    if not rooms[room_id].game_started and not rooms[room_id].matching and len(rooms[room_id].player_names) < len(rooms[room_id].map_details.player_countries):
        add_player_to_room(room_id, len(rooms[room_id].player_names), player_name) #because length of players dictionary is used, the pairs must be immedietely removed upon exiting the game

        return (room_id, len(rooms[room_id].player_names) - 1)

        #returns room
        #print((room_id, len(rooms[room_id].player_names) - 1))
    return -1

#check room assignment
@app.get("/check_room")
async def check_match(room_id: int, player_id: int): #return -1 repeatedly until players are full; then, room json will be returned (needs c# localization)
    try:
        if rooms[room_id].player_names.get(player_id) is None:
            print("player already kicked from server side")
            return -1
        rooms[room_id].player_timeouts[player_id] = 25 #default 15, for testing don't kick
        if len(rooms[room_id].player_names) == len(rooms[room_id].map_details.player_countries):
            #game is ready
            #print("starting game")
            rooms[room_id].game_started = True
            return rooms[room_id].return_value()
        else:
            return 0 #players not full yet; keep checking

    except Exception as e:
        print(str(e))
        #print("attempted to access room that doesn't exist")
        return -1 #room does not exist

        

#players will continuously call this function regarddless of turn
@app.get("/check_turns")
async def check_turns(room_id: int, player_id: int):
    try:
        if rooms[room_id].player_names.get(player_id) is None:
            #print("player already kicked from server side")
            return -1
        rooms[room_id].player_timeouts[player_id] = 75 #default 15
        return rooms[room_id].return_value()
    except:
        #print("wrong room checked")
        return -1

@app.get("/get_map")
async def get_map(room_id:int):
    try:
        return rooms[room_id].map_json
    except:
        return -1
@app.post("/upload_data")
async def upload_map(room_id:int = Form(...), player_id:int = Form(...), json_data:str = Form(...), random_id:str = Form(...), map_view_only:int = Form(...)):
    try:
        if rooms[room_id].current_player == player_id:
            rooms[room_id].map_view_only = map_view_only
            rooms[room_id].map_json = json_data
            rooms[room_id].random_id = random_id
            if not map_view_only: #does not pass round if it is only a syncing map upload
                rooms[room_id].current_player += 1
                if rooms[room_id].current_player == len(rooms[room_id].player_names):
                    rooms[room_id].current_player = 0 #loop back to player 1
            return 0
        else:
            print("not player " + str(player_id) + "\'s turn!")
            return -2
    except:
        return -1
uvicorn.run(app, port=9999, host="0.0.0.0") #new port

