import time, datetime
import Adafruit_DHT
import mysql.connector as mariadb

def establish_sql_connection():
    connection = mariadb.connect(user='sa', password='password', database='RASPBERRY_PI')
    cursor = connection.cursor()
    return connection, cursor

def read_dht_sensor():
    while True:
        humidity, temperature = Adafruit_DHT.read_retry(DHT_SENSOR, DHT_PIN)
        
        if humidity is not None and temperature is not None:
            print("Temp={0:0.1f}*C  Humidity={1:0.1f}%".format(temperature, humidity))
            return round(humidity, 2), round(temperature, 2)
        else:
            print("Failed to retrieve data from humidity sensor")
            return 1000, 1000

def execute_sql_query(sql_cursor, sql_connection, sql_query):
    sql_cursor.execute(sql_query, (humidity, temperature, datetime_now))
    sql_connection.commit()

def reconnect_sql():
    while True:
        try:
            print("Attempting to connect...")
            sql_connection, sql_cursor = establish_sql_connection()
            print("Connection established")
            return sql_connection, sql_cursor
        except:
            time.sleep(10)
            continue

# Main code
DHT_SENSOR = Adafruit_DHT.DHT22
DHT_PIN = 4

sql_connection, sql_cursor = reconnect_sql()
while True:
    try:
        # Read DHT Sensor
        humidity, temperature = read_dht_sensor()

        # Obtain reading datetime
        datetime_now = datetime.datetime.today().replace(microsecond=0)
        #date_now, time_now = date_time_now.date(), date_time_now

        # Insert datetime and sensor readings into SQL database
        sql_query = '''INSERT INTO Dht22SensorData 
        (HumidityReading, TemperatureReading, DateTimeReading) 
        VALUES (%s, %s, %s)'''
        execute_sql_query(sql_cursor, sql_connection, sql_query)
        print("Reading logged successfully")
        
    except:
        # Catch SQL disconnect and reconnect
        sql_connection, sql_cursor = reconnect_sql()
        continue
    
    finally:
        # SQL log rate
        time.sleep(60)
        
        
        


