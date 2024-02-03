#include <SoftwareSerial.h>

SoftwareSerial softSerial(2, 3); // RX, TX

#include "SparkFun_UHF_RFID_Reader.h"
RFID nano;

#define TAG_TIMEOUT 3000 // Timeout for tag detection in milliseconds

struct TagInfo
{
  String epc;
  unsigned long lastDetectionTime;
  TagInfo* next;
};

TagInfo* tagList = nullptr; // List to store tag information
bool scanningEnabled = false; // Flag to control tag scanning

void setup()
{
  Serial.begin(115200);
  while (!Serial);

  if (setupNano(38400) == false)
  {
    Serial.println(F("Module failed to respond. Please check wiring."));
    while (1);
  }

  nano.setRegion(REGION_NORTHAMERICA);

  nano.setReadPower(2600);
  // Max Read TX Power is 27.00 dBm and may cause temperature-limit throttling

  Serial.println(F("Enter 'we are ready to rock' to begin scanning for tags."));
}

void loop()
{
  if (scanningEnabled)
  {
    static unsigned long lastPrintTime = 0;

    if (nano.check() == true)
    {
      byte responseType = nano.parseResponse();

      if (responseType == RESPONSE_IS_TAGFOUND)
      {
        String currentEPC = getEPCAsString();

        // Check if the tag is in the list
        bool found = false;
        TagInfo* currentTag = tagList;
        TagInfo* previousTag = nullptr;

        while (currentTag != nullptr)
        {
          if (currentTag->epc == currentEPC)
          {
            // Tag already in the list, update last detection time
            currentTag->lastDetectionTime = millis();
            found = true;
            break;
          }

          previousTag = currentTag;
          currentTag = currentTag->next;
        }

        if (!found)
        {
          // Tag not in the list, add it and print information
          TagInfo* newTagInfo = new TagInfo;
          newTagInfo->epc = currentEPC;
          newTagInfo->lastDetectionTime = millis();
          newTagInfo->next = nullptr;

          if (previousTag != nullptr)
          {
            previousTag->next = newTagInfo;
          }
          else
          {
            tagList = newTagInfo;
          }

          printTagInfo(currentEPC);
        }
      }

      // Check for tags that timed out
      TagInfo* currentTag = tagList;
      TagInfo* previousTag = nullptr;

      while (currentTag != nullptr)
      {
          if (millis() - currentTag->lastDetectionTime > TAG_TIMEOUT)
          {
              // Tag timed out, remove and print cancel message
              Serial.print(F("cancel["));
              Serial.print(currentTag->epc);
              Serial.print(F("]"));
              Serial.println();

              if (previousTag != nullptr)
              {
                  previousTag->next = currentTag->next;
                  delete currentTag;
                  currentTag = previousTag->next;
              }
              else
              {
                  TagInfo* nextTag = currentTag->next;
                  delete currentTag;
                  currentTag = nextTag;
                  tagList = currentTag;
              }
          }
          else
          {
              // Check if the tag is online
              if (nano.check() == true && nano.parseResponse() == RESPONSE_IS_TAGFOUND)
              {
                  String currentEPC = getEPCAsString();

                  if (currentTag->epc == currentEPC)
                  {
                      // Tag is online, update last detection time
                      currentTag->lastDetectionTime = millis();
                  }
              }

              previousTag = currentTag;
              currentTag = currentTag->next;
          }
      }
    }
  }

  // Check for user input
  if (Serial.available() > 0)
  {
    String userInput = Serial.readStringUntil('\n');
    if (userInput == "we are ready to rock")
    {
      Serial.println(F("Scanning for tags..."));
      scanningEnabled = true;
      nano.startReading(); // Begin scanning for tags
    }
  }
}

void printTagInfo(String epc)
{
  int rssi = nano.getTagRSSI();
  long freq = nano.getTagFreq();
  long timeStamp = nano.getTagTimestamp();
  byte tagEPCBytes = nano.getTagEPCBytes();

  Serial.print(F("epc["));
  for (byte x = 0; x < tagEPCBytes; x++)
  {
    if (nano.msg[31 + x] < 0x10) Serial.print(F("0"));
    Serial.print(nano.msg[31 + x], HEX);
    Serial.print(F(" "));
  }
  Serial.print(F("]"));

  Serial.println();
}

String getEPCAsString()
{
  byte tagEPCBytes = nano.getTagEPCBytes();
  String epc = "";

  for (byte x = 0; x < tagEPCBytes; x++)
  {
    if (nano.msg[31 + x] < 0x10) epc += "0";
    epc += String(nano.msg[31 + x], HEX) + " ";
  }

  return epc;
}

boolean setupNano(long baudRate)
{
  nano.begin(softSerial);

  softSerial.begin(baudRate);
  while (softSerial.isListening() == false);
  while (softSerial.available()) softSerial.read();

  nano.getVersion();

  if (nano.msg[0] == ERROR_WRONG_OPCODE_RESPONSE)
  {
    nano.stopReading();

    Serial.println(F("Module continuously reading. Asking it to stop..."));

    delay(1500);
  }
  else
  {
    softSerial.begin(115200);

    nano.setBaud(baudRate);

    softSerial.begin(baudRate);

    delay(250);
  }

  nano.getVersion();
  if (nano.msg[0] != ALL_GOOD) return (false);

  nano.setTagProtocol();
  nano.setAntennaPort();

  return (true);
}
