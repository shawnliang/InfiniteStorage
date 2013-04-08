MONGOOSE_DIR=../external/mongoose
CFLAGS=	-DUSE_ws_server -W -Wall -I$(MONGOOSE_DIR) -g
CFILES= ws_server.c $(MONGOOSE_DIR)/mongoose.c  
OBJS=$(CFILES:.c=.o)
TARGET=ws_server

all: $(TARGET)

.c.o:
	$(CC) -c $(CFLAGS) $(ADD) $< -o $@

$(TARGET): $(OBJS)
	$(CC) $(CFLAGS) $(OBJS) $(LIBS) $(ADD) -o $@;

run: $(TARGET)
	./$^

clean:
	rm -rf $(TARGET) *.dSYM $(OBJS) *.o
