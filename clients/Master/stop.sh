ps -ef|grep "umi dev"|grep -v grep|awk '{print $2}' |xargs kill
