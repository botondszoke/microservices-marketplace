package com.buyte.EmailAPI.EmailData;

public class EmailSamples {

    public static String getPurchaseSample(PurchaseData data) {
        return String.format("Hi!<br><br>You have successfully purchased the following product(s):<br><br>Name:<b>%s</b><br>", data.getProductName()) +
                String.format("Condition:<b>%s</b><br>Description:<b>%s</b><br><br><b>%s</b> pc(s)<br><br>", data.getProductCondition(), data.getProductDescription(), data.getQuantity()) +
                "Thank you for shopping with us! We are looking forward to see you again.<br><br>BuYTE Team";
    }
}

