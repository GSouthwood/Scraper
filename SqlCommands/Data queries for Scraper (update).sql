/*Get the top 10 flights under $500*/
SELECT TOP 10 * FROM Flight WHERE price < 500

/*Get the top 10 flights under $500 within a specified time range*/
SELECT TOP 10 * FROM Flight WHERE price < 500
AND departure_date >= '2018-03-12' AND return_date <= '2018-03-21'

/*Get the top 20 swell forecasts above 6.5 feet with flights going to
them under $900
*/
SELECT DISTINCT TOP 20 Destination.name, 
Surf.spot_name, Surf.forecast_for_date, 
ROUND(Surf.swell_height_feet, 0) AS avg_surf_height, Surf.log_date,
COUNT(Flight.flight_id) AS num_of_flights FROM Surf
JOIN Destination ON Destination.location_id = Surf.location_id
JOIN Flight ON Flight.airport_code = Destination.airport_code
WHERE swell_height_feet > 5
AND Flight.departure_date > GETDATE() 
AND Surf.forecast_for_date > Flight.departure_date 
AND Surf.forecast_for_date < Flight.return_date
AND price < 500
AND Surf.log_id IN (SELECT TOP 90 Surf.log_id FROM Surf
ORDER BY Surf.log_id DESC)
AND Flight.flight_id IN (SELECT TOP 45 Flight.flight_id FROM Flight
ORDER BY Flight.flight_id DESC)
GROUP BY Destination.name, 
Surf.spot_name, Surf.forecast_for_date, Surf.swell_height_feet, Surf.log_date
ORDER BY Surf.log_date

/*Get the top 10 flights under $900 where swell height is above 6.5 feet
*/
SELECT DISTINCT TOP 30 Destination.name, 
Surf.spot_name, Flight.departure_date, Flight.return_date,
MIN(Flight.price) AS min_flight_price FROM Surf
JOIN Destination ON Destination.location_id = Surf.location_id
JOIN Flight ON Flight.airport_code = Destination.airport_code
WHERE swell_height_feet > 5 
AND price < 500
AND Flight.departure_date > GETDATE() 
AND Surf.forecast_for_date > Flight.departure_date 
AND Surf.forecast_for_date < Flight.return_date
AND Surf.log_id IN (SELECT TOP 90 Surf.log_id FROM Surf
ORDER BY Surf.log_id DESC)
AND Flight.flight_id IN (SELECT TOP 45 Flight.flight_id FROM Flight
ORDER BY Flight.flight_id DESC)
GROUP BY Destination.name, 
Surf.spot_name, Surf.forecast_for_date, Flight.departure_date, Flight.return_date,
Surf.swell_height_feet
ORDER BY min_flight_price ASC;


SELECT * FROM Spot

  