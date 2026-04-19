> test info



test suite: `nbomber_default_test_suite_name`

test name: `nbomber_default_test_name`

session id: `2026-04-13_14-20-48_e8fa22bd`

> scenario stats



scenario: `Test_GetProducts_Good_Async`

  - ok count: `4660`

  - fail count: `6189`

  - all data: `3.729` MB

  - duration: `00:00:28`

load simulations:

  - `inject`, rate: `500`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `10849`, ok = `4660`, RPS = `166.43`|
|latency (ms)|min = `821.94`, mean = `9228.7`, max = `18864.61`, StdDev = `3704.39`|
|latency percentile (ms)|p50 = `9682.94`, p75 = `11722.75`, p95 = `14745.6`, p99 = `16572.42`|
|data transfer (KB)|min = `0.819`, mean = `0.819`, max = `0.819`, all = `3.729` MB|


|step|failures stats|
|---|---|
|name|`global information`|
|request count|all = `10849`, fail = `6189`, RPS = `221.04`|
|latency (ms)|min = `4116.01`, mean = `5125.64`, max = `5781.89`, StdDev = `278.12`|
|latency percentile (ms)|p50 = `5140.48`, p75 = `5349.38`, p95 = `5521.41`, p99 = `5718.02`|


> status codes for scenario: `Test_GetProducts_Good_Async`



|status code|count|message|
|---|---|---|
|OK|4660||
|-101|6189|No connection could be made because the target machine actively refused it. (localhost:7153)|


