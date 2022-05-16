# Mikroszolgáltatásokra épülő konténer-alapú rendszer megvalósítása
BME AUT Önálló laboratórium - 2021/22/2    
Szőke-Milinte Botond - JQ162H   
Konzulens: Dudás Ákos

## A feladat leírása
A feladat célja egy komplex rendszer elkészítése a mikroszolgáltatások architektúra specialitásaira koncentrálva. Az alkalmazás témájának egy piactér (marketplace) rendszert választottam. A rendszerben lehetőség van felhasználókat kezelni, termékeket létrehozni, tárolni, csoportosítani az azonos tulajdonságú termékeket egy kategóriába, a csoportokat eladásra meghirdetni (egyelőre csak fix áron), illetve a termékeket megvásárolni. A sikeres vásárlásról a rendszer email értesítést küld a felhasználónak. Az alkalmazás a későbbiekben számos funkcionalitással bővíthető (pl. termék életútjának követése, vélemények és értékelések írása, rendes fizetési rendszer megvalósítása). A rendszer backendje Docker konténerekben fut, míg a frontend ettől külön található, egy React technológiával megvalósított weboldal.

## Architekturális felépítés
A rendszer elkészítése során számos technológia megismerésére és felhasználására volt szükség. Három saját készítésű mikroszolgáltatás található a rendszerben, emellett a rendszer működéséhez további szolgáltatások futnak konténerekben.

### ProductService
Ez a szolgáltatás felelős a termékek kezeléséért, tulajdonságaik tárolásáért és változtatásaiért. Felépítését tekintve egy egyszerű, háromrétegű alkalmazás backendjéről beszélhetünk, melyben helyet kap az adatelérési réteg, az üzleti logika, illetve REST API-n keresztül érhetőek el a szolgáltatásai. ASP.NET 6 technológiával került megvalósításra. A termékek és termékcsoportok szöveges adatait egy konténerizált MongoDB adatbázisban tárolja, míg a termékekhez tartozó képeket egy konténerizált Azurite emulátor Blob Storage részében. Fontos megjegyezni, hogy az Azurite emulátor rosszul skálázódik, production környezetben nem célszerű alkalmazni. Így a későbbiekben a Microsoft Azure Blob Storage-ra érdemes átállni, mely már jól skálázódik, kényelmesen lehetővé teszi nagyobb bináris adatok felhőben történő tárolását. Emellett, mivel ez a szolgáltatás adminisztrálja egy vásárlás során a termékek tulajdonosának megváltoztatását, ez a szolgáltatás lép kapcsolatba az EmailService-el, amely később email értesítést küld. A REST API Swagger dokumentációja elérhető a http://product.localhost címen.

### SaleService
Ez a szolgáltatás felelős azért, hogy meg lehessen hirdetni egy termékcsoportot eladásra, tárolja az eladáshoz kapcsolódó adatokat. Felépítését tekintve egy egyszerű, háromrétegű alkalmazás backendjéről beszélhetünk, melyben helyet kap az adatelérési réteg, az üzleti logika, illetve REST API-n keresztül érhetőek el a szolgáltatásai. ASP.NET 6 technológiával került megvalósításra. Az eladásokhoz kapcsolódó adatokat egy MongoDB adatbázisban tárolja. A REST API Swagger dokumentációja elérhető a http://sale.localhost címen.

### EmailService
Ez a szolgáltatás felelős azért, hogy előre felvett eseményekről értesítse a felhasználót egy email üzenet formájában. Java nyelven készült, Spring Boot technológiával került megvalósításra. A szolgáltatás két konténerre válik szét. Az EmailAPI konténer egy REST API-t publikál, melyen keresztül fogadja az egyes üzenetekhez tartozó adatokat, elkészíti az email üzenetet majd ezt egy üzenetsorba teszi bele. Az EmailService konténer pedig az előbbi üzenetsort figyeli, és ha üzenetet lát, kiveszi onnan, és elküldi az emailt. Az email küldése a Gmail-en keresztül valósul meg. Az üzenetsor kezelése a RabbitMQ segítségével zajlik, mely szintén egy konténerben fut.

### API Gateway
Az alkalmazásban API Gateway került megvalósításra, így minden szolgáltatás elérhető egy belépési ponton (jelen esetben a http://localhost-on). Ez felelős a http kérések eljuttatásáért az egyes szolgáltatásokhoz, lehetőséget biztosít arra, hogy egyszerűen elérhetőek legyenek a szolgáltatások, elrejti a kliens elől a backend széttagoltságát. Az API Gateway a Traefik szolgáltatással, konténerizáltan került megvalósításra, mellyel egyszerűen, labelek formájában leírhatóak a szükséges szabályok, beállítások. Emellett lehetőséget biztosít middleware-ek beépítésére is az egyes routereken.

### Felhasználókezelés, authentikáció
A felhasználókezelés a konténerizált Keycloak szolgáltatás segítségével történik. Egyszerű regisztrációt tesz lehetővé, és könnyen hozzákapcsolható a frontend kliensekhez (például Reacthoz és Angularhoz is elérhető a keycloak-js nevű segédkönyvtár), így megkíméli a többi szolgáltatást a felhasználókhoz kapcsolódó adatok tárolásától. Lehetőséget biztosít továbbá egyéb Identity Providerekhez történő kapcsolódásra is (Github, Google, Facebook stb.), ezt a lehetőséget azonban nem használtam még ki a projekt során. A szolgáltatás egy konténerizált PostgreSQL adatbázishoz kapcsolódik, ebben tárolja a felhasználókezeléshez kapcsolódó adatokat.
Az authentikáció OAuth protokoll segítségével történik. A frontend kliensnél ezt egyszerűen megoldja a kapcsolódó keycloak-js segédkönyvtár, a backend esetében az authentikáció a Traefikra van bízva. Ehhez minden szolgáltatáshoz, melyekhez kérés érkezhet egy klienstől, egy forwardauth middleware került kapcsolásra, az azonosítást pedig egy traefik-forward-auth konténerizált szolgáltatás felel. A middleware miatt a kérések először ehhez a szolgáltatáshoz érkeznek. Ez megvizsgálja, hogy be van-e lépve már a felhasználó, amennyiben nincs, átirányítja a felhasználót a Keycloak bejelentkező felületére, ahonnan bejelentkezhet. Sikeres bejelentkezés esetén, vagy már eredetileg is bejelentkezett felhasználó esetén csupán hozzáad egy X-Forwarded-User headert a kéréshez, melyben a felhasználó azonosítója található. Így a szolgáltatásokhoz már egy authentikált kérés érkezik, a felhasználó azonosítója pedig nem a kliens alkalmazás által lett megadva.

### Frontend
Az alkalmazás megjelenítéséért egy egyszerű, React alapú webalkalmazás felelős, mely Material UI-t használ az egyes komponensek ízléses formázásához. Lehetőség van minden, a termékekhez kapcsolódó művelet elvégzésére, képek feltöltésére, azok árának meghirdetésére stb. Ez a konténerizált környezettől külön fut, a http://localhost:3000-es linken érhető el. Fontos megjegyezni, hogy a felhasználó termékeinek összesítő táblázatához a MUI DataGridPro komponensét használja, mely fejlesztési célokra ingyenesen használható, production környezetben azonban licensz megvásárlása szükséges (ezt a konzolban megjelenő hibaüzenetekkel is jelzi).

## Elérhetőség, telepítés
A rendszer forráskódja publikusan elérhető a https://github.com/botondszoke/microservices-marketplace repositoryban. A klónozás után mindenképpen érdemes lecserélni az egyes szolgáltatásokhoz beégetett alapértelmezett neveket és jelszavakat, illetve volume-ot csatolni az adatbázisok konténereihez. Ezután a docker-compose fájl futtatható. Fontos megjegyezni, hogy a konténerek indulásakor a traefik-forward-auth konténer azonnal le is áll. Ennek az az oka, hogy bár a keycloak konténer elindult, ahhoz, hogy kommunikálni is lehessen vele, meg kell várni az „Admin console listening on http://127.0.0.1:9990” üzenetet a konténer kimenetén. Ezután ismét elindítva a konténert, a csatlakozás sikeresen megtörténik. Ezt követően elindítható a Frontend is az npm install és npm start parancsok kiadását követően.

## Fejlesztési lehetőségek
A rendszer számos irányban továbbfejleszthető. Funkciók tekintetében érdemes lehet hozzáadni aukciós modult, véleményezési lehetőséget, életút követést és természetesen a megfelelő fizetési rendszert is. Az eladásokat listázó oldalon (és a kapcsolódó backend modulban) célszerű lenne lapozásos rendszert kialakítani, hiszen nagy mennyiségű adat esetén ez jelentősen csökkenti mind a kliens alkalmazás, mind a szolgáltatás terheltségét, továbbá a hálózati kommunikációt is gyorsítja. A rendszerrel való kommunikáció, és a rendszer belső kommunikációja HTTP protokollal történik (kivéve az email szolgáltatás üzenetsoros kommunikációját), ezt mindenképpen érdemes production környezetben HTTPS protokollra cserélni, és érvényes tanúsítványt beszerezni. Emellett, ahogyan az fentebb olvasható, az Azurite emulátort szükséges lecserélni a jól skálázható Azure Blob Storage-ra, illetve licenszt kell vásárolni a MUI DataGridPro komponenshez, amennyiben production környezetbe szeretnénk helyezni a rendszert.

