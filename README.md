"# BlueCat-Api" 

* API 项目目前缺少，日志系统（NLOG,错误邮件提醒，以及日志记录），全局的exception控制，后期会陆续加上。
* 缺少全局的TOKEN 认证的filter后期也会加上，TOKEN 认证目前采用的是webtoken，未建立authorization service ,目前不考虑重写一套基于.NET的  auth2.0的框架，
  如果有兴趣可以参考https://github.com/DotNetOpenAuth/DotNetOpenAuth

* API  简介：Repository 主要是负责数据读取以及持久化，不对数据做任何转换，Repository 可以是redis，mogodb的，SQL；service层是对数据做处理的加工厂，并返回API所需要的数据！
* service层是对数据做处理的加工厂，处理多个Repository 返回的结果集，并返回API所需要的数据！
* BlueCat.Api.Entity 是一个SQL的EF 的工作单元，包装了对EF的操作，供Repository 调用！
* 如果有对REDIS以及其他数据库做操作，也建议去新建项目，参考BlueCat.Api.Entity
* 项目参数认证采用FluentValidation，准备优化方向为：https://github.com/jasonmitchell/fluentvalidation-webapi-autofac/tree/master/Sample.Web 或者https://github.com/richardlawley/WebApi-FluentValidation
* ABP写API也是一种好的方式，目前暂时没有去考虑用这个去写，后期会新建一个项目用ABP框架去写API.
* 其他待补充！如果有其他疑问联系我：707907488@qq.com


