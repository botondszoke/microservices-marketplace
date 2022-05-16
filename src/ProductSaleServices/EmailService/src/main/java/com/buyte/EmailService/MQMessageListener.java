package com.buyte.EmailService;

import org.springframework.amqp.rabbit.annotation.RabbitListener;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Configuration;

@Configuration
public class MQMessageListener {
    @Autowired
    private EmailSenderService senderService;

    @RabbitListener(queues = MQConfig._queue)
    public void listener(MQMessage message) {
        System.out.println(message);
        sendMail(message.getToEmail(), message.getSubject(), message.getBody());
    }

    public void sendMail(String toEmail, String subject, String body) {
        senderService.sendEmail(toEmail, subject, body);
    }
}
