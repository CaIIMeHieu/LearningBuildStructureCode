> test info



test suite: `nbomber_default_test_suite_name`

test name: `nbomber_default_test_name`

session id: `2026-04-13_14-21-25_27afb0f0`

> scenario stats



scenario: `Test_GetProducts_Bad_Sync`

  - ok count: `1788`

  - fail count: `1`

  - all data: `1.429` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `500`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `1789`, ok = `1788`, RPS = `59.6`|
|latency (ms)|min = `5201.95`, mean = `51718.36`, max = `89191.54`, StdDev = `21045.89`|
|latency percentile (ms)|p50 = `53477.38`, p75 = `65142.78`, p95 = `84082.69`, p99 = `88997.89`|
|data transfer (KB)|min = `0.818`, mean = `0.818`, max = `0.818`, all = `1.429` MB|


|step|failures stats|
|---|---|
|name|`global information`|
|request count|all = `1789`, fail = `1`, RPS = `0.03`|
|latency (ms)|min = `1345.95`, mean = `1345.95`, max = `1345.95`, StdDev = `0.1`|
|latency percentile (ms)|p50 = `1346.56`, p75 = `1346.56`, p95 = `1346.56`, p99 = `1346.56`|


> status codes for scenario: `Test_GetProducts_Bad_Sync`



|status code|count|message|
|---|---|---|
|OK|1788||
|-101|1|An operation on a socket could not be performed because the system lacked sufficient buffer space or because a queue was full. (localhost:7153)|


