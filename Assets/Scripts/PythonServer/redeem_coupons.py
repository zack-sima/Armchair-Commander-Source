#coding=utf-8
from fastapi import FastAPI
import time
from starlette.websockets import WebSocket
import uvicorn
import sys

app = FastAPI()
from fastapi.middleware.cors import CORSMiddleware


server_password = ["redacted"]
origins = [
    "*"
]

app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)
redeem_codes = {}
@app.get("/redeem_coupon")
async def redeem_coupon(coupon_code: str):
    amount = 0
    if redeem_codes.get(coupon_code):
        amount = redeem_codes[coupon_code]
    redeem_codes[coupon_code] = 0
    return amount
@app.get("/check_coupons")
async def check_coupons(password: str):
    if password == server_password[0]:
        return redeem_codes
    else:
        return "wrong password"
@app.get("/add_coupon")
async def add_coupon(coupon_code: str, coupon_value: int, password: str):
    if password == server_password[0]:
        redeem_codes[coupon_code] = coupon_value
        return "successfully added " + str(coupon_code) + " to coupon bank"
    else:
        return "wrong password"
uvicorn.run(app, port=9999, host="0.0.0.0")

