# -*- coding: utf8 -*-
#Dependancies: pymysql, sqlalchemy, (mysql-connector?), mysql-server (apt), pydantic (internal)
from pydantic import BaseModel
from fastapi import Query
import asyncio
import sqlalchemy.ext.declarative
import sqlalchemy.orm
import sqlalchemy.orm.session
import sqlalchemy
import datetime
import threading
import os
import time
import random

MYSQL_URL = "mysql+mysqlconnector://myuser:mypassword@localhost:3306/redacted"
BASE= sqlalchemy.ext.declarative.declarative_base()


class Map(BASE):
    __tablename__ = "uploadData"
    uid = sqlalchemy.Column(sqlalchemy.BIGINT, primary_key=True)
    author =sqlalchemy.Column(sqlalchemy.String)
    json_file = sqlalchemy.Column(sqlalchemy.String)
    map_name=sqlalchemy.Column(sqlalchemy.String)
    completed_count=sqlalchemy.Column(sqlalchemy.Integer, default=0)
    views=sqlalchemy.Column(sqlalchemy.BIGINT, default=0)
    rank_score=sqlalchemy.Column(sqlalchemy.FLOAT, default=0)


engine = sqlalchemy.create_engine(MYSQL_URL, encoding="utf8", echo=False)
sqlalchemy.orm.session.Session = sqlalchemy.orm.sessionmaker(bind=engine)

indexer = 0
def rank_score_interval():
    global indexer
    while True:
        print(indexer)
        try:
            if not update_rank_score():
                indexer += 100
            else:
                indexer = 0
        except Exception as e:
            print(e)
            time.sleep(200) #probably timed out
        time.sleep(10)

def update_rank_score(): #also filters out ', ", and \ marks in the names
    session = sqlalchemy.orm.session.Session()
    allrows = session.query(Map.views, Map.completed_count, Map.uid)

    highest_index_id = int(allrows.order_by(Map.uid.desc())[0].uid)
    rows_total_length = allrows.count()

    global indexer

    rows = session.query(Map.views, Map.completed_count, Map.uid, Map.author, Map.map_name, Map.rank_score).offset(indexer).limit(105)
    exceeded = False

    for index, i in enumerate(rows.all()):
         #0 = views, 1 = likes, 2 = id (inverse upload time)

        views = i[0]
        likes = i[1]
        map_id = i[2]

        #print(f"row: {map_id}, {views}")
        if i[5] == -1: #skip through censored maps so they stay the same score
            continue

        if map_id < highest_index_id - 15000 and views < 10: #delete maps older than xxx id's from the newest with little amount of views
            print(f"deleted row: {map_id}")
            indexer -= 1
            allrows.filter_by(uid=i[2]).delete() #CAUTION: will delete the row forever
            
        else:
            #print(f"updated row: {map_id}")
            #views beyond 1500 and likes beyond 350 don't matter in calculation;
            views = min(1500, views)
            likes = min(350, likes)


            score = ((views+10)**0.35)*((likes + 2)/(views + 10)+0.25)/((highest_index_id-map_id+100)**0.5)*random.uniform(0.7, 1.25)

            #all featured maps have to have a minimum number of views
            if views < 5:
                score = 0
            elif views < 10:
                score *= 0.7

            #having too many views will also reduce points
            if views > 500:
                score *= 0.5
            elif views > 1000:
                score *= 0.2
            elif views > 1500:
                score = 0


            new_name = i[4].replace("\'", "")
            new_name = new_name.replace("\"", "")
            new_name = new_name.replace("\\", "")
            new_author = i[3].replace("\'", "")
            new_author = new_author.replace("\"", "")
            new_author = new_author.replace("\\", "")

            allrows.filter_by(uid=i[2]).update({"rank_score": score, "map_name": new_name, "author": new_author})
    if rows_total_length <= indexer + 100:
        exceeded = True
    session.commit()
    session.close()
    return exceeded

threading.Thread(target=rank_score_interval).start()