#include <DS2431.h>
#include <OneWire.h>
#define FAST 20
#define MEDIUM 50
#define SLOW 100


const int ONE_WIRE_PIN = 9;
const int LD_PIN = 6;
bool blink = false;
int period = 50;
bool state = HIGH;
byte buffer[1024] = { 0 };
char command = ' ';
OneWire oneWire(ONE_WIRE_PIN);
DS2431 eeprom(oneWire);

void onTimer() {
  if (blink) {
    digitalWrite(LD_PIN, state);
    state = !state;
  } else {
    digitalWrite(LD_PIN, HIGH);
  }
}
ISR(TIMER2_COMPA_vect) {  // timer compare interrupt service routine
  static int counter = 0;
  if (++counter >= period) {
    counter = 0;
    onTimer();
  }
}

void setup() {
  // put your setup code here, to run once:
  pinMode(LD_PIN, OUTPUT);
  Serial.begin(115200);
  cli();                                 // disable all interrupts
  TCCR2A = (1 << WGM21) | (0 << WGM20);  // Mode CTC
  TIMSK2 = (1 << OCIE2A);                // Local interruption OCIE2A
  TCCR2B = (0 << WGM22) | (1 << CS22) | (1 << CS21);
  OCR2A = 250;
  sei();
  blink = true;
  period = SLOW;
  while (!Serial) {
  }


  blink = false;
}

void loop() {
  // put your main code here, to run repeatedly:
  while (!oneWire.reset()) {
    blink = true;
    period = MEDIUM;
  }
  blink = false;
  while (Serial.available() <= 0) {
  }
  Serial.readBytesUntil("\n",&command,1);
  
  blink = true;
  period = FAST;
  switch (command) {
    case 'r':
      eeprom.read(0, buffer, sizeof(buffer));
      for (int i=0; i<sizeof(buffer);i++) {
        Serial.print((char)buffer[i]);
      }
      
      break;
    case 'w':
      Serial.readBytesUntil("\n",buffer,sizeof(buffer));
      Serial.flush();
      eeprom.write(0, buffer, 0x8F,true);
      Serial.print('1');
      break;
  }
  blink=false;
  Serial.flush();
}
