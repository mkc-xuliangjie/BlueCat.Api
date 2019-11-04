API 项目模板，简单三层。

Mongodb 来自于：https://github.com/YGeneral/PoJun.MongoDB

logEvent.TimeStamp.ToString()+logEvent.TimeStamp.ToString()


db.getCollection('CustomLog').find({Message:{$regex:"\"TraceId\":\"b8030612-4b61-4e1f-a6f8-9371d90803e3\""}})