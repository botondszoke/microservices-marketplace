package com.buyte.EmailAPI;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.ToString;

import java.util.Date;

@Data
@NoArgsConstructor
@AllArgsConstructor
@ToString
public class MQMessage {
    private String messageId;
    private Date messageDate;

    private String toEmail;
    private String subject;
    private String body;
}
