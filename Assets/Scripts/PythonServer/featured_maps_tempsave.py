import requests
import time

while True:
	try:
		featured_maps = requests.get("http://retrocombat.com:9999/get_all_featured", timeout=10)

		if featured_maps.text != "\"\"" and featured_maps.text != "":
			f = open("featured_maps_savedlist.txt", "w")
			f.write(featured_maps.text)
			f.close()
		else:
			f = open("featured_maps_savedlist.txt", "r")
			delta_featured = f.read()
			f.close()
			#this means featured is dead and needs to be reuploaded
			reupload_featured = delta_featured.replace(" \\\\ ", " \\ ").replace("\"", "")[:-3]
			requests.get(f"http://retrocombat.com:9999/add_featured?names_and_authors={reupload_featured}&password=retrocombat_admin", timeout=10)
			#print(f"http://retrocombat.com:8080/add_featured?names_and_authors={reupload_featured}&password=retrocombat_admin")
	except:
		pass
	time.sleep(1000)