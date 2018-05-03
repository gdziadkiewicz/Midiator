/*
* Arduino Wireless Communication Tutorial
*     Example 1 - Transmitter Code
*                
* by Dejan Nedelkovski, www.HowToMechatronics.com
* 
* Library: TMRh20/RF24, https://github.com/tmrh20/RF24/
*/
#include "SPI.h"
#include "RF24.h"
#include "nRF24L01.h"
#include "Arduino.h"


RF24 radio(7, 8); // CE, CSN

const byte address[6] = "00001";
void setup() {
  Serial.begin(38400);
  radio.begin();
  radio.openReadingPipe(0, address);
  radio.setPALevel(RF24_PA_MIN);
  radio.startListening();
}

void loop() {
  if (radio.available()) {
    char text[128] = "";
    radio.read(&text, sizeof(text));
    Serial.println(text);
  }
}