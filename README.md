# GUAPSchedulerParser
Parser of GUAP Scheduler for some project

### `parser.cs`
#### `struct Lession`
Struction for keeping data about lession
##### Public fields
|Name|Type|Description|
|-----|------|------|
|`time`|`string` (in format `"{number of lecture} пара (HH:mm-HH:mm)"`|lecture time, number of lecture, starting lecture, ending lecture |
|`place`|`string`|lecture audience|
|`name`|`string`|name of the lecture subject|
|`type`|`string` (one of `"ПР"`, `"Л"`, `"ЛР"`)|type of lecture, (practical work, lecture, laboratory work)|
|`isOnUpWeek`|`bool`|`true`, if there is a lecture on the upper weeks (the first week is upper, then lower, then upper, and so on)|
|`isOnDownWeek`|`bool`|`true`, if there is a lecture on the lower weeks (the first week is upper, then lower, then upper, and so on)|

##### Public methods
|Name|Arguments|Returns|Description|
|-----|------|------|------|
|`Print`|none|none|Write to console data about lesstion in format `"{time} - {name}\n"`|

#### `struct DaySchedule`
Struction for keeping data about lessions of some day
##### Public fields
|Name|Type|Description|
|-----|------|------|
|`weekday`|`string`|weekday in Russian, first letter is capital|
|`lessions`|`List<Lession>`|list of lessions of some daye|

##### Public methods
|Name|Arguments|Returns|Description|
|-----|------|------|------|
|`Print`|none|none|Write to console data about day lesstions in format `"{weekday}\n{lession1}\n{...}"`|


#### `struct Schedule`
Struction for keeping data about schedule
##### Public fields
|Name|Type|Description|
|-----|------|------|
|`days`|`List<DaySchedule>`|list of `DaySchedule` of each days|
|`out_grid`|`DaySchedule`|lessions that have not own day|

##### Public methods
|Name|Arguments|Returns|Description|
|-----|------|------|------|
|`Print`|none|none|Write to console data about schedule in format `"{out_grid}\n{day1}\n{...}"`|




#### `class Parser`
Sttatic class that contain methods for parsing SUAI web-site
##### Public fields
|Name|Type|Description|
|-----|------|------|
|`DEFAULT_URL`|`const string`|URL of SUAI group schedule web-site|

##### Public methods
|Name|Arguments|Returns|Description|
|-----|------|------|------|
|`getHTML`|`string`|`Task<string>`|Get URL SUAI group schedule web-site and download schedule in HTML code |
|`ParseHtml`|`string`|`Schedule`|Creates schedule data structures and writes data to them from the resulting HTML code|
|`GetSchedule`|`string`|`Schedule`|Get URL SUAI group schedule web-site (if havent, use `DEFAULT_URL`) and results schedule of group in `Schedule` structure|

#### `class Program`
Main class of programm, using for testing, parse SUAI group schedule web-site and write it to console
##### Public methods
|Name|Arguments|Returns|Description|
|-----|------|------|------|
|`Main`|none|none|Entry point to the program, parse SUAI group schedule web-site and write it to console|
