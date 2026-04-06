// WebAuthn helpers for passkey login and registration.
// Fido2NetLib serializes all binary fields as base64url strings; the browser
// WebAuthn API requires ArrayBuffers. These helpers handle the conversion.

function base64UrlToBytes(base64url) {
    const base64 = base64url.replace(/-/g, '+').replace(/_/g, '/');
    const padded  = base64.padEnd(base64.length + (4 - base64.length % 4) % 4, '=');
    const binary  = atob(padded);
    return Uint8Array.from(binary, c => c.charCodeAt(0));
}

function bytesToBase64Url(buffer) {
    const bytes  = buffer instanceof ArrayBuffer ? new Uint8Array(buffer) : buffer;
    let   binary = '';
    bytes.forEach(b => (binary += String.fromCharCode(b)));
    return btoa(binary).replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
}

// Calls navigator.credentials.get() using the AssertionOptions JSON returned
// by the server. Returns a JSON string ready to POST to the complete endpoint.
async function startPasskeyAuthentication(optionsJson) {
    const opts = JSON.parse(optionsJson);

    const publicKey = {
        challenge:          base64UrlToBytes(opts.challenge),
        rpId:               opts.rpId,
        timeout:            opts.timeout,
        userVerification:   opts.userVerification,
        allowCredentials:   (opts.allowCredentials || []).map(c => ({
            id:         base64UrlToBytes(c.id),
            type:       c.type,
            transports: c.transports
        }))
    };

    const credential = await navigator.credentials.get({ publicKey });

    const assertion = {
        id:                    credential.id,
        rawId:                 bytesToBase64Url(credential.rawId),
        type:                  credential.type,
        clientExtensionResults: credential.getClientExtensionResults(),
        response: {
            authenticatorData: bytesToBase64Url(credential.response.authenticatorData),
            clientDataJSON:    bytesToBase64Url(credential.response.clientDataJSON),
            signature:         bytesToBase64Url(credential.response.signature),
            userHandle:        credential.response.userHandle
                                   ? bytesToBase64Url(credential.response.userHandle)
                                   : null
        }
    };

    return JSON.stringify(assertion);
}

// Calls navigator.credentials.create() using the CredentialCreateOptions JSON
// returned by the server. Returns a JSON string ready to POST to the complete endpoint.
async function startPasskeyRegistration(optionsJson) {
    const opts = JSON.parse(optionsJson);

    const publicKey = {
        challenge:            base64UrlToBytes(opts.challenge),
        rp:                   opts.rp,
        user: {
            id:               base64UrlToBytes(opts.user.id),
            name:             opts.user.name,
            displayName:      opts.user.displayName
        },
        pubKeyCredParams:     opts.pubKeyCredParams,
        timeout:              opts.timeout,
        excludeCredentials:   (opts.excludeCredentials || []).map(c => ({
            id:         base64UrlToBytes(c.id),
            type:       c.type,
            transports: c.transports
        })),
        authenticatorSelection: opts.authenticatorSelection,
        attestation:          opts.attestation,
        extensions:           opts.extensions
    };

    const credential = await navigator.credentials.create({ publicKey });

    const attestation = {
        id:                    credential.id,
        rawId:                 bytesToBase64Url(credential.rawId),
        type:                  credential.type,
        clientExtensionResults: credential.getClientExtensionResults(),
        response: {
            attestationObject: bytesToBase64Url(credential.response.attestationObject),
            clientDataJSON:    bytesToBase64Url(credential.response.clientDataJSON),
            transports:        credential.response.getTransports
                                   ? credential.response.getTransports()
                                   : []
        }
    };

    return JSON.stringify(attestation);
}
