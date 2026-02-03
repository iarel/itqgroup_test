Client Name + Count of Contacts

SELECT
    c.ClientName,
    COUNT(cc.Id) AS ContactCount
FROM Clients c
LEFT JOIN ClientContacts cc
    ON cc.ClientId = c.Id
GROUP BY c.ClientName;



Clients with more than 2 contacts
SELECT
    c.ClientName
FROM Clients c
JOIN ClientContacts cc
    ON cc.ClientId = c.Id
GROUP BY c.ClientName
HAVING COUNT(cc.Id) > 2;
