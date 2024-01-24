# OData

Alle Controller im Ordner ODataController sind "ODataController" (Basisklasse Microsoft.AspNetCore.OData.Routing.Controllers.ODataController).
Diese Basisklasse ermöglicht Requests nach dem OData (Open Data Protocol) Standard (https://www.odata.org).

Die Endpunkte (Routen) können hierbei standardisiert "erweitert" werden mit zusätzlichen Parametern in der URL.


## Beispiele:
normale Route:  https://localhost:5001/api/odata/geo/area/47.83768/16.251904/5000/-1
mit erweiterten Parametern: https://localhost:5001/api/odata/geo/area/47.83768/16.251904/5000/-1?$filter=id ge 151
Zu beachten ist hier dass am Ende ein zusätzlicher Parameter ("?$filter=id ge 151") angegeben wird der nicht in der "normalen" Endpunkt-Definition definiert ist.
OData-Parameter beginnend hier immer mit dem $-Zeichen (diese Einstellung könnte man allerdings ändern, sodass die Parameter auch ohne vorangestelltes $-Zeichen angegeben werden können).
Der Filter im o.a. Beispiel filtert die Response (vorab auf dem Server) nach allen Elementen mit der Eigenschaft "id" größer oder gleich ("ge" für greater or equal) wie 151 ist.
Weitere filter-Abfrage-Optionen: https://learn.microsoft.com/en-us/odata/webapi/first-odata-api#filter

Ein anderes Beispiel wäre: https://localhost:5001/api/odata/geo/area/47.83768/16.251904/5000/-1?$select=id
Hier bewirkt der Parameter "$select=id", dass nur die Ids der Elemente gesendet werden. Hier können natürlich nur Eigenschaften angegeben werden, die in den Response-Elementen vorhanden sind, ansonsten wird eine entsprechende Fehlermeldung retourniert.
Mehrere Eigenschaften können mittel Beistrich getrennt werden -> "$select=id,distance,value"

Die Abfrage Parameter können auch aneinander gereiht werden -> siehe ein allgemeines Beispiel hier: https://learn.microsoft.com/en-us/odata/webapi/first-odata-api#chained-queries

Parameter Expand ("$expand=..."):
Man kann die Abfragen auch erweitern mittels "$epand=..." erweitern. Beispiel: https://localhost:5001/api/odata/measurementdefinitions?$expand=tblMeasurements
Dadurch werden auch alle Messresulte (tblMeasurements) in der Response inkludiert.
Der Expand-Parameter kann auch verschachtelt werden. Beispiel: "$expand=tblmeasurementdefintions($expand=tblMeasurements)".
In IXchange werden wir die Expand-Funktion allerdings einschränken, um zu verhindern, dass vom User nicht freigegebene Daten auch nicht über Umwege (Verschachtelung von Expand-Parameter) abgefragt werden können. 

## Allgemein
Um die OData-Schnittstelle anzupassen, hier ein hilfreicher Link: https://learn.microsoft.com/en-us/odata/webapi-8/overview (sowie die Links darunter im Menü auf der Webseite)