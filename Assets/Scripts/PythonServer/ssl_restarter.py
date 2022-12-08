import os
import subprocess
import time
import requests
import threading

def start_process(directory):
	subprocess.run(("nohup python3 " + directory + " &").split(" "))

def kill_process(process_name):
	out = os.popen("ps aux | grep " + process_name).read()
	if ".py" in out:
		print(out)
		s = out.split(" ")
		for i in s:
			try:
				test_code = int(i)
				os.system("kill -9 " + i)
				break
			except: #tries to parse everything
				...

while True:
	print("restarting at " + str(time.asctime()))
	try:
		kill_process("online_players_tracker")
		threading.Thread(target=start_process, args=("/root/server/online_players_tracker.py",)).start()
	except:
		print("restart failed: online_players_tracker")

	try:
		kill_process("mapwars_server_https")
		threading.Thread(target=start_process, args=("/root/mapwars/mapwars_server_https.py",)).start()
	except:
		print("restart failed: mapwars_server_https")

	time.sleep(864000) #every day
		