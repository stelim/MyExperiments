# MyExperiments

Just a place for my experiments with various topics like

* Paket
* RabbitMQ
* Topshelf
* Quartz
* ...

Don't expect something useful! 
You have been warned!

## TopRabbit

Send a message to a RabbitMQ queue each second after service is started.
When service is stopping, the queue will be emptied.

Requirements:
Running RabbitMQ instance on your machine.


Install as Service and start it.
	
	.\TopRabbit.exe install --sudo
	.\TopRabbit.exe {start|stop} and visit http://localhost:15672/#/queues/%2F/hello

![RabbitMQ Screenshot](rabbitmq.png)

