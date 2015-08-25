#!/usr/bin/python
import time
import datetime

i = datetime.datetime.now()
with open('//../BenMAP-CE/BenMAP/Resources/BuildDate.txt','w') as f:
    f.write ( time.strftime("%B") + " %s, %s" %(i.day,i.year))


