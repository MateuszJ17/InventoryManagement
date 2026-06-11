<h2>Assumptions and simplifications:</h2>

<ul>
<li>App does not use a Result pattern. I worked with the assumption that client of this api would not normally
pass in nonsense data like a productId that does not exist in the system. Thus if that would happen I treat it
as a legitimate exceptional situation that should not happen and that warrants an exception.</li>
<li>In the same spirit, if product stock is too low it is handled by returning null from the command handler.
If I'd use a Result pattern this would be handled by returning a Result.Failure.</li>
<li>For the bank holidays I've skipped the ones with a date that is not set in stone, like corpus christi.</li>
<li>Regional price multiplier works of a set two char string found in the Customer entity.</li>
<li>Since regional price multiplier for US is basically 1.0 I've just specified the ones for the EU and Asia
and lump US with the default case of no multiplier.</li>
<li>Connection strings are found in the appsettings/docker compose files. In a production envrionment stuff like
that would be stored in a secure secrets store like Key Vault or GH secrets.</li>
<li>App uses only console serilog sink. I think that is enough for this use case.</li>
</ul>

<h2>Tradeoffs:</h2>
<ul>
<li>For integration tests for the sake of simplicity I'm using an in memory database.
This is not ideal and I would not use that in a production environment,
instead I would use something like MySql or a dedicated postgres.</li>
<li>I'm not using a combo of Dapper/EF for data access. I don't think that in our usecase the performance gain
is worth it.</li>
<li>There is a race condition in the CreateOrderCommandHandler that
could manifest if two orders for the same product are placed at the same
,or similar, time. For the sake of simplicity I've ignored it but in a real envrionment
it should use a concurrency token to solve this.</li>
<li>I'm using MediatR for CQRS. After the licensing changes this could
be problematic and might be replaced with something else or custom implementation.</li>
</ul>