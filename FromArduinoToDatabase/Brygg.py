#!/usr/bin/python

import serial 
import MySQLdb
import datetime
import time
import ast
import sys
import smtplib

from email.MIMEMultipart import MIMEMultipart
from email.MIMEText import MIMEText
from datetime import datetime

databaseTabell = 'bryggtest'

#Mail setup
fromaddr = 'ospe.bryggeri@gmail.com'
toaddrs  = ['sindrerhansen@gmail.com', 'bjorndalseter@gmail.com']
msg = MIMEMultipart()
msg['From'] = fromaddr
msg['To'] = ", ".join(toaddrs)
server = smtplib.SMTP('smtp.gmail.com', 587)
server.ehlo()
server.starttls()
server.ehlo()
server.login("ospe.bryggeri@gmail.com", "ospe1948")


#establish connection to MySQL. You'll have to change this for your database.
dbConn = MySQLdb.connect("localhost","sindre","Kaffe123","bryggelogg") or die ("could not connect to bryggelogg database")
#open a cursor to the database
cursor = dbConn.cursor()

#establish connection to MySQL. You'll have to change this for your database.
dbConn2 = MySQLdb.connect("localhost","sindre","Kaffe123","arduino") or die ("could not connect to arduino database")
#open a cursor to the database
cursor2 = dbConn2.cursor()

connected = False
lastState = False
writtenValue= [0.0] * 10
device = 'COM16' #this will have to be changed to the serial port you are using
timez = time.time()
timez2 = time.time()
#sqlSessionData = "SELECT * FROM current_brew_session WHERE idcom = 1"

try:
  print "Trying...",device , "At:" ,datetime.now().strftime('%Y-%m-%d %H:%M:%S')
  arduino = serial.Serial(device, 9600)
  connected = True
except: 
  print "Failed to connect on",device    
if connected:
  print "Connected"
#  arduino.write("prepere\r")

while connected: 
   # if ((timez2 + 2) < time.time()):
   #   timez2 = time.time()
   #   cursor2.execute(sqlSessionData)
   #   allData = cursor2.fetchone() 
   #   if allData[1]==1:
   #     arduino.write("prepare\n".encode())

    data = arduino.readline()  #read the data from the arduino
    pieces = data.split("\t")  #split the data by the tab
    print "At: ",datetime.now().strftime('%Y-%m-%d %H:%M:%S'), "\n", "Added Volume:", pieces[0], "L, HWT Temp:", pieces[1], "C", "Mash Tank Temp:", pieces[2], "C", "Boil Tank Temp:", pieces[3], "C", "State:", pieces[4], "Info:", pieces[6]

    addedVolume = ast.literal_eval(pieces[0].strip())
    hwtTemp = ast.literal_eval(pieces[1].strip())
    mashTankTemp = ast.literal_eval(pieces[2].strip())
    boilTankTemp = ast.literal_eval(pieces[3].strip())
    state = ast.literal_eval(pieces[4].strip())
    
    if ((timez + 300) < time.time()):
      addToDatabase = True
      timez = time.time()

    for x in range(0, 3):
      stemp =pieces[x].strip()
      ftemp = ast.literal_eval(stemp)
      dif = ftemp - writtenValue[x]
      dif = abs (dif)
 #     print " Diff er ", dif
      if dif > 0.3:
        writtenValue[x]=ftemp
        addToDatabase = True

    if addToDatabase:
      try:
        sql = "INSERT INTO bryggtest (addedVolume, hwtTemp, mashTankTemp, boilTankTemp, state) VALUES (%f, %f, %f, %f, %f)"  %  (addedVolume, hwtTemp, mashTankTemp, boilTankTemp, state)      
        cursor.execute(sql)
        dbConn.commit()
        print "Inserted data to database"
      except: 
        print "Failed to insert into database"
      addToDatabase = False



