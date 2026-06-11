<h1>Inventory Management</h1>

<h3> How to run api locally with db in docker: </h3>

<ul>
<li>start the db by using docker-compose up postgres</li>
<li>wait until the db is initialized and ready to accept connections</li>
<li>run the api using dotnet run normally</li>
</ul>

<h3> How to run with Docker: </h3>

<ul>
<li>run docker-compose up</li>
<li>everything should spin up by itself and be exposed on port 8080 (http)</li>
</ul>


<h3>Api endpoints</h3>
<br/>
<span>Api endpoints include:</span>
<ul>
<li>GET /products</li>
<li>POST /products</li>
<br/>
<li>POST /orders</li>
</ul>

<span>Open API document available under <b>/openapi/v1.json</b></span>