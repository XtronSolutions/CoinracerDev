mergeInto(LibraryManager.library, {

    CreateUserWithEmailAndPassword: function (email, password, objectName, callback, fallback) {
        var parsedEmail = Pointer_stringify(email);
        var parsedPassword = Pointer_stringify(password);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            firebase.auth().createUserWithEmailAndPassword(parsedEmail, parsedPassword).then(function (unused) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: signed up for " + parsedEmail);
            }).catch(function (error) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    SignInWithEmailAndPassword: function (email, password, objectName, callback, fallback) {
        var parsedEmail = Pointer_stringify(email);
        var parsedPassword = Pointer_stringify(password);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            firebase.auth().signInWithEmailAndPassword(parsedEmail, parsedPassword).then(function (unused) {
			console.log("Success: signed in for " + parsedEmail);
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, unused.user.uid);
            }).catch(function (error) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    SignInWithGoogle: function (objectName, callback, fallback) {
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {
            var provider = new firebase.auth.GoogleAuthProvider();
            firebase.auth().signInWithRedirect(provider).then(function (unused) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: signed in with Google!");
            }).catch(function (error) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    SignInWithFacebook: function (objectName, callback, fallback) {
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {
            var provider = new firebase.auth.FacebookAuthProvider();
            firebase.auth().signInWithRedirect(provider).then(function (unused) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: signed in with Facebook!");
            }).catch(function (error) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    OnAuthStateChanged: function (objectName, onUserSignedIn, onUserSignedOut) {
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedOnUserSignedIn = Pointer_stringify(onUserSignedIn);
        var parsedOnUserSignedOut = Pointer_stringify(onUserSignedOut);

		//AuthenticateAnonymous(parsedObjectName,parsedOnUserSignedIn,parsedOnUserSignedOut);
        firebase.auth().onAuthStateChanged(function(user) {
            if (user) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedOnUserSignedIn, JSON.stringify(user));
            } else {
                unityInstance.Module.SendMessage(parsedObjectName, parsedOnUserSignedOut, "User signed out");
            }
        });
    },
	
	AuthenticateAnonymous: function (objectName, callback, fallback) {
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);
		
		try {
            firebase.auth().signInAnonymously().then(function () {
				console.log("Anonymous login was successful");
                //unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Anonymous Auth done");
            }).catch(function (error) {
                console.log(JSON.stringify(error, Object.getOwnPropertyNames(error)));
				//unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
                console.log(JSON.stringify(error, Object.getOwnPropertyNames(error)));
				//unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },
	
	CheckEmail: function (emailAdress,objectName, callback, fallback) {
		var parsedEmail = Pointer_stringify(emailAdress);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);
		
		try {
            firebase.auth().fetchSignInMethodsForEmail(parsedEmail).then(function (signInMethods) {
			if(signInMethods.length>0)
			{
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Email registered");
			}else
			{
				unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, "Email Not Registered");
			}
            }).catch(function (error) {
				unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
				unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },
	
	SignOut: function (objectName, callback, fallback) {
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);
		
		try {
            firebase.auth().signOut().then(function () {
              unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Signed Out");
			  console.log('Signed Out');
            }).catch(function (error) {
				unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });
        } catch (error) {
				unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },
	
	SendEmailVerification: function (objectName, callback, fallback) {
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);
		
		var actionCodeSettings = {
            url: window.location.href,
            handleCodeInApp: true,
        };
		
		try {
			 firebase.auth().currentUser.sendEmailVerification(actionCodeSettings)
            .then(function () {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Email Verification sent");
            })
            .catch(function (error) {
				unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));

            });
        } catch (error) {
				unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },
	
	CheckEmailVerification: function (objectName, callback, fallback) {
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);
		
		var _isVerified=firebase.auth().currentUser.emailVerified;
		unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(_isVerified));
    },
	
	SendPasswordResetEmail: function (ObjectEmail,objectName, callback, fallback) {
        var parsedEmailName = Pointer_stringify(ObjectEmail);
		var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);
		
		try {
			 firebase.auth().sendPasswordResetEmail(parsedEmailName)
            .then(function () {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Email Reset sent");
            })
            .catch(function (error) {
				unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));

            });
        } catch (error) {
				unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    }
});
