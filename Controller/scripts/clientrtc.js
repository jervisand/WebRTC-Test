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
			urls: "stun:openrelay.metered.ca:80",
		},
		{
			urls: "turn:openrelay.metered.ca:80",
			username: "openrelayproject",
			credential: "openrelayproject",
		},
		{
			urls: "turn:openrelay.metered.ca:443",
			username: "openrelayproject",
			credential: "openrelayproject",
		},
		{
			urls: "turn:openrelay.metered.ca:443?transport=tcp",
			username: "openrelayproject",
			credential: "openrelayproject",
		},
	],
}

export function Initialize(){
	localConnection = new RTCPeerConnection();
	dataChannel = localConnection.createDataChannel("channel");
	
	localConnection.onicecandidate = e => {
		console.log("New Ice Candidate! Print SDP: " + JSON.stringify(localConnection.localDescription));
		runtime.globalVars.sdp = JSON.stringify(localConnection.localDescription);
	};
	
	dataChannel.onmessage = e => console.log("Message Entered: " + e.message);
	dataChannel.onopen = e => console.log("Channel Connected!");
	
	CreateOffer();
}

export function CreateOffer()
{
	if(localConnection != null)
	{
		localConnection.createOffer()
			.then(o => localConnection.setLocalDescription(o))
			.then(console.log("Offer created!"));
	}
}

export function SetRemoteDescription(data)
{
	localConnection.setRemoteDescription(JSON.parse(data));
}

export function SendMessage(data)
{
	dataChannel.send(data);
}

export function AddIceCandidate(data)
{
	localConnection.addIceCandidate(offer);
}