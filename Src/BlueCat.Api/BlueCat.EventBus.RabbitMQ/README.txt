
1.配置文件：RabbitMQ消息队列连接配置信息

  

"RabbitMQEventBus": {
                "HostName": "",
                "UserName": "",
                "Password": "",
                "ClientName": "",
                "ExchangeName": ""
        }


  
2）添加依赖注入信息（鼠标移上去点击黑箭头可以自动添加命名空间的引用）
   （模块文件.cs）
  
public override void ServicesInitialize(IServiceCollection services,)
                {
                        var hostName = Configuration.GetSection("RabbitMQEventBus:HostName").Value;
                        var userName = Configuration.GetSection("RabbitMQEventBus:UserName").Value;
                        var password = Configuration.GetSection("RabbitMQEventBus:Password").Value;
                        var clientName = Configuration.GetSection("RabbitMQEventBus:ClientName").Value;
                        var exchangeName = Configuration.GetSection("RabbitMQEventBus:ExchangeName").Value;
                        services.AddRabbitMQEventBus((e) =>
                        {
                        },
                        (e) =>
                        {
                                e.ConnectionFactories.Default = new ConnectionFactory()
                                {
                                        HostName = hostName,
                                        UserName = userName,
                                        Password = password
                                };
                        },
                        (e) =>
                        {
                                e.ClientName = clientName;
                                e.ExchangeName = exchangeName;
                        });
                }


3）编写发布事件
      
//发布 Rabbitmq消息



                                        var bus = IServiceProvider.GetService<IEventBus>();
                                         

                                        bus.PublishAsync<WaringEvent>(new WaringEvent()
                                        {
                                                EventID = DateTime.Now.Ticks.ToString(),
                                                model = new Model()
                                        });

  4）发布事件参数类定义
      
public class WaringEvent
{
		/// <summary>
		/// 事件ID(追溯事件唯一标识)
		/// </summary>
		public string EventID { get; set; }
		/// <summary>
		/// 模型
		/// </summary>
		public Model model{ get; set; }
}
复制代码

3.订阅事件模块

  1）引用BlueCat.EventBus.RabbitMQ （跟发布事件模块一样）
  2）订阅事件编写（模块文件.cs）
      
public override void RegisterComplated(IServiceProvider serviceProvider)
                {
                        base.RegisterComplated(serviceProvider);

                        var bus = (IEventBus)serviceProvider.GetService(typeof(IEventBus));

                        if (bus != null)
                        {
                                var businessServices = serviceProvider.GetService<BusinessServices>();
                                //订阅监测预警事件
                                bus.Subscribe<WaringEvent>((data) =>
                                {
                                       // to do something
									    //businessServices.doSomething();
                                        return Task.CompletedTask;
                                });
                        }

                }