#!/usr/bin/python

import serial 
import MySQLdb
import datetime
import time
import matplotlib.pyplot as plt
import matplotlib.animation as animation
import ast
import wx

#establish connection to MySQL. You'll have to change this for your database.
dbConn = MySQLdb.connect("localhost","sindre","Kaffe123","fermentation_logg") or die ("could not connect to database")
cursor = dbConn.cursor()
connected = False
writtenValue= 0.0
device = 'COM10' #this will have to be changed to the serial port you are using
timez = time.time()
try:
  print "Trying...",device 
  sensor = serial.Serial(port = device, baudrate=115200, bytesize=serial.EIGHTBITS, parity=serial.PARITY_NONE, stopbits=serial.STOPBITS_ONE)
  connected = True

except: 
  print "Failed to connect on",device

while connected: 
    from datetime import datetime
    sensor.write('R')
    stemp = sensor.read(6)
    stemp = stemp.strip()
    ftemp = ast.literal_eval(stemp) - 0.8
    dif=ftemp - writtenValue
    dif = abs (dif)
    print "At: ",datetime.now().strftime('%Y-%m-%d %H:%M:%S'), "\n", "Temp is: ", ftemp
    print "Diff er ", dif;
    diftime = timez + 60 - time.time()
    print "Time conting down ", diftime
    if dif>0.2 or (timez + 60) < time.time():
      timez = time.time()
      writtenValue=ftemp
      print "Adding value ", ftemp, " to database"
      sql = "INSERT INTO fermentation2_temp (temp) VALUES (%f)"  %  (ftemp)      
      try:
        cursor.execute(sql)
        dbConn.commit() #commit the insert
      except:
        print "SQL error"

 
