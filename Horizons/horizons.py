import numpy as np
import pandas as pd
import time, urllib.request, os.path
from datetime import datetime, date, timedelta
import calendar
import os
def get_size(start_path = '.'):
    total_size = 0
    for dirpath, dirnames, filenames in os.walk(start_path):
        for f in filenames:
            fp = os.path.join(dirpath, f)
            # skip if it is symbolic link
            if not os.path.islink(fp):
                total_size += os.path.getsize(fp)

    return total_size


def isNaN(string):
    return string != string

def get_link(objid, stepsize, center):
    start_date = datetime.now()
    days_in_month = calendar.monthrange(start_date.year, start_date.month)[1]
    end_date = start_date + timedelta(days=days_in_month*12)
    link = "https://ssd.jpl.nasa.gov/horizons_batch.cgi?batch=1"
    link += "&COMMAND='"+ str(objid) +"'"
    link += "&COORD_TYPE='GEODETIC'"
    link += "&SITE_COORD='+151.28330,-33.91660,0'"
    link += "&MAKE_EPHEM='YES'"
    link += "&TABLE_TYPE='ELEMENTS'"
    link += "&START_TIME='"+ start_date.strftime("%Y-%m-%d") +"'"
    link += "&STOP_TIME='"+ end_date.strftime("%Y-%m-%d") +"'"
    link += "&STEP_SIZE='"+ stepsize + "'"
    link += "&OUT_UNITS='AU-D'"
    link += "&REF_SYSTEM='J2000'"
    link += "&TP_TYPE='ABSOLUTE'"
    link += "&ELEM_LABELS='YES"
    link += "&OBJ_DATA='YES'"
    link += "&CSV_FORMAT='YES'"
    link += "&CENTER='"+ center + "'"
    return(link)

def get_data(link, savename, objid, name):
    #values = {'name':'Lewis Ham', 'location':'Sydney NSW AU', 'language':'Python 3.7.1'}
    head = { 'User-Agent':"Space Antenna Bot" }
   # data = urllib.parse.urlencode(values)
   # data = data.encode('utf-8')
    
    
    if not os.path.isfile(savename):
        try:
            req = urllib.request.Request(link, headers=head)
            opener = urllib.request.build_opener()
            output = opener.open(req)
            #output = urllib.request(link, data, headers)
            output = [x.decode('UTF-8') for x in output]
            output = [x.strip() for x in output]

            if '$$SOE' in output:
                header = output[output.index('$$SOE')-2].split(',')
                header = [x.lstrip() for x in header]

                content = output[output.index('$$SOE')+1 : output.index('$$EOE')-1]
                content = [x.split(',') for x in content]

                df = pd.DataFrame(content, columns=header)
                name_list = name
                df['name'] = name_list
                df.to_csv(savename, index=False)
            else: 
                print(objid, 'request successful but output not expected format')
                print(link)
        except Exception as e:
            print(e)
            print(objid, 'request unsuccessful')
            print(link)


def query_horizons(readname, savename_head, stepsize='1d', center='@sun'):
    if not os.path.isfile(savename_head[:-1]+".zip"):
        print("#####################################################################")
        print("Now analyzing", readname)
        df = pd.read_csv(readname, low_memory=False)

        # Check for duplicated names before running script
        dupl = df.duplicated('horizons')
        print(sum(dupl), 'names in the series are duplicated, of', len(df), 'total')
        if sum(dupl) > 1:
            print(df[dupl == True])

        # Check to see if any names are NaN values
        print(df['horizons'].astype(str).replace(' ', '').isnull().sum(), 
                  'null values in horizons query list')

        print("index".ljust(10), "name".ljust(15), "ID".ljust(15), "Status & size")
        for index, row in df.iterrows():
            if (index % 500 == 0) and (index != 0):
                print(index, 'items analyzed!')
            objid = str(row['horizons']).replace(' ', '')
            name = str(row['name']).replace(' ', '')
            savename = savename_head + objid + '.csv'

            if (not os.path.isfile(savename)) and not ( name == '' or pd.isnull(name) or name == 'nan'):
                link = get_link(objid, stepsize, center)
                get_data(link, savename, objid, name)
                try:
                    print(str(index + 1).ljust(10), name.ljust(15), objid.ljust(15), 'DATA RETRIEVED, ', os.path.getsize(savename)/1000, ' Kb');
                except:
                    print("Not saved")
                # sleep to be polite to HORIZONS servers
                #sleeptime = min(20, max(1, np.random.normal(loc=10, scale=5)))
                #time.sleep(sleeptime) 

                if index == 0:
                    # Save parameters of 1st item to txt file for later checking
                    txtname = savename_head + 'PARAMETERS.txt'
                    with open(txtname, "w") as f:
                        f.write(link)
    else: 
        raise ValueError("Please unzip the data files that already exist")
    print("---------------------------------------------------------------------")
    print('ALL ITEMS ANALYZED! Folder size :: ', get_size(savename_head)/1000000, 'Mbytes')


def yes_or_no(question):
    while "the answer is invalid":
        reply = str(input(question+' (y/n): ')).lower().strip()
        if reply[0] == 'y':
            return True
        if reply[0] == 'n':
            return False


if yes_or_no("Delete previous data?"):
    for f in os.listdir('./data/moons/'):
        if not f.endswith(".csv"):
            continue
        os.remove(os.path.join('./data/moons/', f))
    for f in os.listdir('./data/planets/'):
        if not f.endswith(".csv"):
            continue
        os.remove(os.path.join('./data/planets/', f))
    for f in os.listdir('./data/large_asteroids/'):
        if not f.endswith(".csv"):
            continue
        os.remove(os.path.join('./data/large_asteroids/', f))
    for f in os.listdir('./data/large_comets/'):
        if not f.endswith(".csv"):
            continue
        os.remove(os.path.join('./data/large_comets/', f))
    for f in os.listdir('./data/small_asteroids/'):
        if not f.endswith(".csv"):
            continue
        os.remove(os.path.join('./data/small_asteroids/', f))
    for f in os.listdir('./data/any_outer_asteroids/'):
        if not f.endswith(".csv"):
            continue
        os.remove(os.path.join('./data/any_outer_asteroids/', f))
    for f in os.listdir('./data/any_inner_asteroids/'):
        if not f.endswith(".csv"):
            continue
        os.remove(os.path.join('./data/any_inner_asteroids/', f))



center = '@sun'
stepsize = '1d'

readname = './data/moons.data'
savename_head = './data/moons/'
query_horizons(readname, savename_head, stepsize=stepsize, center=center)

readname = './data/planets.data'
savename_head = './data/planets/'
query_horizons(readname, savename_head, stepsize=stepsize, center=center)

readname = './data/large_asteroids.data'
savename_head = './data/large_asteroids/'
query_horizons(readname, savename_head, stepsize=stepsize, center=center)

readname = './data/large_comets.data'
savename_head = './data/large_comets/'
query_horizons(readname, savename_head, stepsize=stepsize, center=center)

readname = './data/small_asteroids.data'
savename_head = './data/small_asteroids/'
query_horizons(readname, savename_head, stepsize=stepsize, center=center)

readname = './data/any_outer_asteroids.data'
savename_head = './data/any_outer_asteroids/'
query_horizons(readname, savename_head, stepsize=stepsize, center=center)

readname = './data/any_inner_asteroids.data'
savename_head = './data/any_inner_asteroids/'
query_horizons(readname, savename_head, stepsize=stepsize, center=center)