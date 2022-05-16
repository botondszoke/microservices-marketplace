package com.buyte.EmailService;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.stereotype.Service;

import javax.mail.internet.MimeMessage;

@Service
public class EmailSenderService {
    @Autowired
    private JavaMailSender mailSender;

    public void sendEmail(String toEmail, String subject, String body) {
        MimeMessage mimeMessage = mailSender.createMimeMessage();
        MimeMessageHelper helper = new MimeMessageHelper(mimeMessage, "utf-8");
        String htmlMsg = body;
        try {
            helper.setFrom("info.buyte@gmail.com");
            helper.setTo(toEmail);
            helper.setText(htmlMsg, true);
            helper.setSubject(subject);
        }
        catch (Exception e) {
            System.out.println(e.getMessage());
        }
        mailSender.send(mimeMessage);
        System.out.println("Email successfully sent");
    }
}
