#!/usr/bin/python
import time
import datetime
import os
import sys


basepath = sys.argv[1]
os.chdir(basepath)

i = datetime.datetime.now()


filename ='BuildDate.txt'
#print "Date updated in: " + filename
with open(filename,'w') as f:
    f.write ( time.strftime("%B") + " %s, %s" %(i.day,i.year))


