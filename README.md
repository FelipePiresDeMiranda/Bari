# Bari

Projeto de micro serviço desenvolvido para consumo da plataforma AWS SNS SQS utilizando .Net Core 3.1 e linguagem C#

Observações:

1) Necessida realizar refactoring para criação de interfaces e injeção de dependência.

2) Os projetos de produção e consumo de mensagens utilizam o serviço da amazon para produzir uma fila de mensagens por isso há necessidade de uma conta na Amazon AWS para utilizá-lo. 

3) Criei classes que utilizam Kafka como serviço de mensageria mas a forma como este serviço interage com o AWS precisa ser estruturada melhor.
