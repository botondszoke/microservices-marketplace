package com.buyte.EmailAPI;

import com.buyte.EmailAPI.EmailData.EmailSamples;
import com.buyte.EmailAPI.EmailData.PurchaseData;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import java.util.Date;
import java.util.UUID;

@RestController
public class EmailController {

    @Autowired
    private RabbitTemplate template;

    @PostMapping("/purchaseMail")
    public String publishMessage(@RequestBody PurchaseData data) {
        MQMessage message = new MQMessage();
        message.setMessageId(UUID.randomUUID().toString());
        message.setMessageDate(new Date());
        message.setSubject("Thank you for your purchase!");
        message.setToEmail(data.getToEmail());
        message.setBody(EmailSamples.getPurchaseSample(data));

        template.convertAndSend(MQConfig._exchange, MQConfig._routing_key, message);

        return "Message published!";
    }
}
