package com.buyte.EmailAPI.EmailData;

public class EmailSamples {

    public static String getPurchaseSample(PurchaseData data) {
        return String.format("Hi!\n\nYou have successfully purchased the following product(s):\n\nName:<b>%s</b>\n", data.getProductName()) +
                String.format("Condition:<b>%s</b>\nDescription:<b>%s</b>\n<b>%s</b> pc(s)\n", data.getProductCondition(), data.getProductDescription(), data.getQuantity()) +
                String.format("Thank you for shopping with us! We are looking forward to see you again.\n\nBuYTE Team");
    }
}

