
void setup() {
  Serial.begin(115200);
  
  pinMode(13, OUTPUT);
  
  establishContact();
}

void establishContact() {
  while (Serial.available() <= 0) {
    digitalWrite(13, HIGH);
    delay(100);
    Serial.println("c:0"); 
    int command = Serial.read(); 
    if(command=='>'){
       digitalWrite(13, HIGH);
       Serial.println("ACK"); 
    }
    
    digitalWrite(13, LOW);  
    delay(100);
    
  }
}

void loop() {
  
 if(Serial.available()) 
  {
    int command = Serial.read(); 
    
    if(command=='1'){
       digitalWrite(13, HIGH);
       Serial.println("ON");
    }
    if(command=='0'){
       digitalWrite(13, LOW);
       Serial.println("OFF");
    }

    if(command=='2')
    {
      for(int i=0;i<100;i++){
        Serial.println(String(i));
        digitalWrite(13, HIGH);
        delay(100);
        digitalWrite(13, LOW);
        delay(100);
        }
      }
  }
    
}



