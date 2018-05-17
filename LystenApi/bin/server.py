import os
from flask import Flask, request
from twilio.jwt.access_token import AccessToken
from twilio.jwt.access_token.grants import VoiceGrant
from twilio.rest import Client
from twilio.twiml.voice_response import VoiceResponse

ACCOUNT_SID =  'AC58a8f470d6f9178a7d2c82250c42d26c'      #'AC58a8f470d6f9178a7d2c82250c42d26c' #'ACba66147727f38172a069d8611ab20232'
API_KEY = 'SK0b6501b76c46dd8b11232185179428d2'			 #'SK0b6501b76c46dd8b11232185179428d2'#'SK68e953efe34ba88af56a123121b49409'
API_KEY_SECRET = 'hlNUVOmxFVfeKVgDSMFNB8w0qVXV0Fjb'		   #'pvNEpRmAfYYZcOAgio3N4BAtXSJfQqM1'#'Q9Mz2mUKrwebFVZZ5aNaKQjceybFCl8V'
PUSH_CREDENTIAL_SID = 'CRf61c38471becd71e879ab0e5b8c7e9c4' #'CRbaedf8cab9c19b4c89fe94b491093d2b'#'CRd28452ec07bcdaebd4b0c82c991cd011' 
APP_SID = 'AP7a09a85e33b24cd083da6534c39255e8'            #'AP7a09a85e33b24cd083da6534c39255e8'#'APec4e960c89ca43ea8dec92c9a0ee61f6'

"""
Use a valid Twilio number by adding to your account via https://www.twilio.com/console/phone-numbers/verified
"""
CALLER_NUMBER = '+919727446563'

"""
The caller id used when a client is dialed.
"""
CALLER_ID = 'client:quick_start'
IDENTITY = 'alice'


app = Flask(__name__)

"""
Creates an access token with VoiceGrant using your Twilio credentials.
"""
@app.route('/accessToken')
def token():
  account_sid = os.environ.get("ACCOUNT_SID", ACCOUNT_SID)
  api_key = os.environ.get("API_KEY", API_KEY)
  api_key_secret = os.environ.get("API_KEY_SECRET", API_KEY_SECRET)
  push_credential_sid = os.environ.get("PUSH_CREDENTIAL_SID", PUSH_CREDENTIAL_SID)
  app_sid = os.environ.get("APP_SID", APP_SID)

  grant = VoiceGrant(
    push_credential_sid=push_credential_sid,
    outgoing_application_sid=app_sid
  )

  token = AccessToken(account_sid, api_key, api_key_secret, identity=IDENTITY)
  token.add_grant(grant)

  return str(token)

"""
Creates an endpoint that plays back a greeting.
"""
@app.route('/incoming', methods=['GET', 'POST'])
def incoming():
  resp = VoiceResponse()
  resp.say("Congratulations! You have received your first inbound call! Good bye.")
  return str(resp)

"""
Makes a call to the specified client using the Twilio REST API.
"""
@app.route('/placeCall', methods=['GET', 'POST'])
def placeCall():
  account_sid = os.environ.get("ACCOUNT_SID", ACCOUNT_SID)
  api_key = os.environ.get("API_KEY", API_KEY)
  api_key_secret = os.environ.get("API_KEY_SECRET", API_KEY_SECRET)

  client = Client(api_key, api_key_secret, account_sid)
  call = client.calls.create(url=request.url_root + 'incoming', to='client:' + IDENTITY, from_='client:' + CALLER_ID)
  return str(call.sid)

"""
Creates an endpoint that can be used in your TwiML App as the Voice Request Url.

In order to make an outgoing call using Twilio Voice SDK, you need to provide a
TwiML App SID in the Access Token. You can run your server, make it publicly
accessible and use `/makeCall` endpoint as the Voice Request Url in your TwiML App.
"""
@app.route('/makeCall', methods=['GET', 'POST'])
def makeCall():
  response = VoiceResponse()
  to = request.form.get('to')
  if not to or len(to) == 0:
    response.say("Congratulations! You have just made your first call! Good bye.")
  elif to[0] in "+918905796980":
    response.dial(callerId=CALLER_NUMBER).number(to)
  else:
    response.dial(callerId=CALLER_ID).client(to)
  return str(response)

@app.route('/', methods=['GET', 'POST'])
def welcome():
  resp = VoiceResponse()
  resp.say("Welcome to Twilio")
  return str(resp)

if __name__ == "__main__":
  port = int(os.environ.get("PORT", 5001))
  app.run(host='192.168.1.95', port=port, debug=True)
