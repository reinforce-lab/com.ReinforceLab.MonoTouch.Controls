# MonoTouch make file 
# rev 001 2009/10/23 A. Uehara

include config.make

# definitions
MT_TOOL_DIR = /Developer/MonoTouch/usr/bin
MT_LIB_DIR  = /Developer/MonoTouch/usr/lib/mono/2.1

CSC         = $(MT_TOOL_DIR)/smcs
MTOUCH      = $(MT_TOOL_DIR)/mtouch
MIBTOOL     = $(MT_TOOL_DIR)/mibtool
ARM_MONO    = $(MT_TOOL_DIR)/arm-darwin-mono
MONO        = $(MT_TOOL_DIR)/mono
MTOUCH_PACK = $(MT_TOOL_DIR)/mtouchpack
XCODEBUILD  = /usr/bin/xcodebuild
XCODE       = /Developer/Applications/Xcode.app/Contents/MacOS/Xcode

TOOL_DIR    = /usr/bin
RESGEN      = $(TOOL_DIR)/resgen
AL          = $(TOOL_DIR)/al

COMPILE_TARGET = exe

_pwd        = $(shell pwd)
_info_plist = $(APP_DIR)/Info.plist

# -- debug bild for an iphone simulator --
ifeq ($(CONFIG), DebugiPhoneSimulator)
 BUILD_DIR    = $(_pwd)/bin/iPhoneSimulator/Debug
 CSC_FLAGS    = -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
 ASSEMBLY_MDB = $(ASSEMBLY).mdb
 MTOUCH_FLAGS   = -v --debug --nomanifest --nosign -sim "$(APP_DIR)" 
endif

# -- release build for an iphone simulator
ifeq ($(CONFIG), ReleaseiPhoneSimulator)
 BUILD_DIR    = $(_pwd)/bin/iPhoneSimulator/Release
 CSC_FLAGS    = -noconfig -codepage:utf8 -warn:4 -optimize-
 ASSEMBLY_MDB = 
 MTOUCH_FLAGS   = -v --nomanifest --nosign -sim "$(APP_DIR)" 
endif

# -- debug bild for an iphone
ifeq ($(CONFIG), DebugiPhone)
 BUILD_DIR    = $(_pwd)/bin/iPhone/Debug
 CSC_FLAGS    = -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
 ASSEMBLY     = bin/iPhoneStimulator/Debug/Sample_ScrollView.exe
 ASSEMBLY_MDB = $(ASSEMBLY).mdb
 MTOUCH_FLAGS   = -v --nomanifest --dev "$(APP_DIR) --certificate=$(CERTIFICATION) 
endif

# -- release build for an iphone
ifeq ($(CONFIG), ReleaseiPhone)
 BUILD_DIR    = $(_pwd)/bin/iPhone/Release
 CSC_FLAGS    = -noconfig -codepage:utf8 -warn:4 -optimize-
 ASSEMBLY_MDB = 
 MTOUCH_FLAGS   = -v --nomanifest --dev "$(APP_DIR)" --certificate=$(CERTIFICATION) 
endif

ASSEMBLY     = $(BUILD_DIR)/$(ASSEMBLY_NAME).$(COMPILE_TARGET)
APP_DIR      = $(BUILD_DIR)/$(ASSEMBLY_NAME).app
APP_ASSEMBLY = $(APP_DIR)/$(ASSEMBLY_NAME).$(COMPILE_TARGET)
CLEANFILES   = $(APP_DIR) $(ASSEMBLY)

build_references  := $(foreach item, $(REFERENCES), "/r:$(item).dll")
mtouch_references := $(foreach item, $(REFERENCES), -r="$(MT_LIB_DIR)/$(item).dll")
copy_contents := $(foreach item, $(EXTRAS), $(APP_DIR)/$(item) )

XCODE_PROJECT_DIR  = $(BUILD_DIR)/XcodeProject
XCODE_PROJECT_NAME = $(XCODE_PROJECT_DIR)/$(ASSEMBLY_NAME).xcodeproj

xcode_debug_resource_references := $(foreach item, $(EXTRAS),     -res="$(APP_DIR)/$(item)","$(item)") 
xcode_debug_lib_references      := $(foreach item, $(REFERENCES), -r"$(MT_LIB_DIR)/$(item).dll")


define _do_copy_contents
$1 : $2
	@echo "Copying '$$<' to '$$@'"
	@mkdir -p '$$(shell dirname '$$@')'
	cp '$$<' '$$@'
# 	@test -z '$$<' || cp '$$<' '$$@'

endef


# rules
all:  $(copy_contents) $(APP_ASSEMBLY) $(_info_plist)
	@echo done.
	
$(APP_ASSEMBLY): $(ASSEMBLY)
	mkdir -p $(APP_DIR)
	$(MTOUCH) $(MTOUCH_FLAGS) $(mtouch_references) "$(ASSEMBLY)"

$(ASSEMBLY): $(FILES) 
	mkdir -p $(shell dirname $(ASSEMBLY)) 
	$(CSC) $(CSC_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_references) $(FILES)

$(eval $(foreach item, $(EXTRAS), $(call _do_copy_contents, $(APP_DIR)/$(item), $(item) )))

clean:
	-rm -Rf $(CLEANFILES)

run: all
#	$(shell ps -A | grep iPhoneSimulator | grep -v grep | awk '{print $1}' | xargs kill)
	rm -f stderr.log stdout.log;
	$(MTOUCH) -launchsim=$(APP_DIR) -stderr=stderr.log -stdout=stdout.log;

debug-xcode: all
	mkdir -p $(XCODE_PROJECT_DIR)
	$(MTOUCH) -v -xcode="$(XCODE_PROJECT_DIR)" $(xcode_debug_resource_references) $(xcode_debug_lib_references) "$(ASSEMBLY)"
#	@echo Launching Xcode
#	$(XCODE) $XCODE_PROJECT_NAME)
	
$(_info_plist): config.make
	mkdir -p $(shell dirname $(_info_plist)) 
	$(shell echo "<?xml version="1.0" encoding=\"utf-8\"?>" > $(_info_plist))
	$(shell echo "<!DOCTYPE plist PUBLIC \"-//Apple Computer//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">" >> $(_info_plist))
	$(shell echo "<plist version=\"1.0\">" >> $(_info_plist))
	$(shell echo "  <dict>" >> $(_info_plist))

	$(shell echo "<key>CFBundleDevelopmentRegion</key><string>English</string>" >> $(_info_plist))
	$(shell echo "<key>CFBundleDisplayName</key><string>$(ASSEMBLY_NAME)</string>" >> $(_info_plist))
	$(shell echo "<key>CFBundleExecutable</key><string>$(ASSEMBLY_NAME)</string>" >> $(_info_plist))
	$(shell echo "<key>CFBundleIdentifier</key><string>com.yourcompany.$(ASSEMBLY_NAME)</string>" >> $(_info_plist))
	$(shell echo "<key>CFBundleInfoDictionaryVersion</key><string>6.0</string>" >> $(_info_plist))
	$(shell echo "<key>CFBundleName</key><string>$(ASSEMBLY_NAME)</string>" >> $(_info_plist))
	$(shell echo "<key>CFBundlePackageType</key><string>APPL</string>" >> $(_info_plist))
	$(shell echo "<key>CFBundleSignature</key><string>????</string>" >> $(_info_plist))
	$(shell echo "<key>CFBundleSupportedPlatforms</key><array><string>iphonesimulator</string></array>" >> $(_info_plist))
	$(shell echo "<key>CFBundleVersion</key><string>1.0</string>" >> $(_info_plist))
	$(shell echo "<key>DTPlatformName</key><string>iphonesimulator</string>" >> $(_info_plist))
	$(shell echo "<key>DTSDKName</key><string>iphonesimulator3.0</string>" >> $(_info_plist))
	$(shell echo "<key>LSRequiresIPhoneOS</key><true />" >> $(_info_plist))

	$(shell echo "  </dict>" >> $(_info_plist))
	$(shell echo "</plist>" >> $(_info_plist))
