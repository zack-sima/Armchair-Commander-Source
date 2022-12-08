import os
import subprocess
import time
import requests
import threading

def process():
	subprocess.run("nohup python3 armchair_server.py > ss.out &".split(" "))
while True:
	time.sleep(250)
	try:	
		requests.get("http://retrocombat.com:8080/docs", timeout=25)
		f = open("ss.log", "w")
		f.write("success " + time.asctime())
		f.close()
	except:
		f = open("ss.log", "w")
		f.write("failure " + time.asctime())
		f.close()
		out = os.popen("ps aux | grep armchair_server").read()
		if ".py" in out:
			s = out.split(" ")
			for i in s:
				try:
					code = int(i)
					if (code != 0):
						os.system("kill -9 " + str(code))
						break
				except:
					...
		threading.Thread(target=process).start()
		