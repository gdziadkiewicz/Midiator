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
//#define DEBUG

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
    uint16_t buffer[6] = {0};
    radio.read(&buffer, sizeof(buffer)*2);
    
    for(size_t i = 0; i < 12; i++)
    {
      Serial.write(((uint8_t*)buffer)[i]);
    }
    
    #ifdef DEBUG
    char text[128] = "";
    sprintf(text, "a/g:\t%d\t%d\t%d\t%d\t%d\t%d", buffer[0],buffer[1],buffer[2],buffer[3],buffer[4],buffer[5]);
    Serial.println(text);
    #endif
  }
}