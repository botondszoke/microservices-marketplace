package com.buyte.EmailAPI.EmailData;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Data
@NoArgsConstructor
@AllArgsConstructor
@ToString
public class PurchaseData {
    private String productName;
    private String productCondition;
    private String productDescription;
    private String quantity;

    private String toEmail;
}
