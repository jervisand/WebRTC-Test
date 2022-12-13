let runtime;

runOnStartup(async runtime =>
{
	runtime.addEventListener("beforeprojectstart", () => OnBeforeProjectStart(runtime));
});

async function OnBeforeProjectStart(run)
{
	// Now the runtime is ready, get the TextInput and ResultText instances
	runtime = run;
}

export let localConnection = null;
export let dataChannel;

export const iceConfiguration = {
	iceServers: [
		{
			urls: "stun:stun1.l.google.com:19302",
		}
	]
}

export function Initialize(){
	localConnection = new RTCPeerConnection(iceConfiguration);
	dataChannel = localConnection.createDataChannel("channel");
	
	localConnection.onicecandidate = e => {
		if(e.candidate != null){
			console.log("New Ice Candidate! Print SDP: " + e.candidate);
			runtime.callFunction("ChangeOfferText", [JSON.stringify(localConnection.localDescription)]);
		}
		runtime.callFunction("SendCandidate", [JSON.stringify(e.candidate)])
	};
	
	dataChannel.onmessage = e => {
		runtime.callFunction("AppendPongMessage", e.data, Date.now());
		console.log("Message Entered: " + e.data);
	};
	dataChannel.onopen = e => {
		runtime.callFunction("ClearLog");
		console.log("Channel Connected!");
	};
	
	CreateOffer();
}

export function CreateOffer()
{
	if(localConnection != null)
	{
		localConnection.createOffer()
			.then(o => {
				localConnection.setLocalDescription(o);
				runtime.callFunction("SendOffer", [JSON.stringify(o)])
			})
			.then(console.log("Offer created!"));
	}
}

export function SetRemoteDescription(data)
{
	localConnection.setRemoteDescription(JSON.parse(data));
}

export function SendMessage(data)
{
	runtime.globalVars.recordTime = Date.now();
	dataChannel.send(data);
}

export function AddIceCandidate(data)
{
	console.log("Add A Candidate! " + data);
	localConnection.addIceCandidate(JSON.parse(data));
}

export function GetLocalDescription(){
	return JSON.stringify(localConnection.localDescription);
}

export function GetDate(){
	return Date.now();
}