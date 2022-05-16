package com.buyte.EmailService;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.context.event.ApplicationReadyEvent;
import org.springframework.context.event.EventListener;

@SpringBootApplication
public class EmailServiceApplication {
	@Autowired
	private EmailSenderService senderService;
	public static void main(String[] args) {
		SpringApplication.run(EmailServiceApplication.class, args);
	}

	/*@EventListener(ApplicationReadyEvent.class)
	public void sendMail() {
		senderService.sendEmail("simph007@gmail.com", "Another heads up pt 3", "Hi! Just <b>checking</b> to see if <i>everything</i> is okay.");
	}*/
}
