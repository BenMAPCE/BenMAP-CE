# A distutils script to make a standalone .exe of BuildDate.py  for
# Windows platforms.  You can get py2exe from
# http://py2exe.sourceforge.net/.  Use this command to build the .exe
# and collect the other needed files:
#
#       python setup.py py2exe --excludes=Image
#
"""
__version__ = "$Revision: 1.0 $"
__date__ = "$Date: 2015/07/24 16:37:37 $"
"""

from distutils.core import setup
import py2exe

py2exe.options={"packages" : "encodings"}

setup( name = "BuildDate",
    	console = ['BuildDate.py'],
		options = {'py2exe': {'bundle_files':1,'compressed':True}},
	zipfile=None
       )

