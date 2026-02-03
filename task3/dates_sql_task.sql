SELECT
    d1.Id,
    d1.Dt AS Sd,
    MIN(d2.Dt) AS Ed
FROM Dates d1
JOIN Dates d2
    ON d2.Id = d1.Id
   AND d2.Dt > d1.Dt
GROUP BY d1.Id, d1.Dt
ORDER BY d1.Id, d1.Dt;